using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BankingApp.Data;
using BankingApp.Models;
using Microsoft.AspNetCore.Authorization;

namespace BankingApp.Controllers
{
    [Authorize]
    public class BankAccountsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BankAccountsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: BankAccounts
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.BankAccounts.Include(b => b.Owner);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: BankAccounts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts
                .Include(b => b.Owner)
                .Include(b => b.Operations)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // GET: BankAccounts/Create
        public IActionResult Create()
        {
            ViewData["OwnerId"] = new SelectList(_context.ApplicationUsers, "Id", "Id");
            return View();
        }

        // POST: BankAccounts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Balance,CreditLimit,OwnerId")] BankAccount bankAccount)
        {
            if (ModelState.IsValid)
            {
                _context.Add(bankAccount);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", bankAccount.OwnerId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (bankAccount == null)
            {
                return NotFound();
            }
            ViewData["OwnerId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", bankAccount.OwnerId);
            return View(bankAccount);
        }

        // POST: BankAccounts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Balance,CreditLimit,OwnerId")] BankAccount bankAccount)
        {
            if (id != bankAccount.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(bankAccount);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BankAccountExists(bankAccount.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["OwnerId"] = new SelectList(_context.ApplicationUsers, "Id", "Id", bankAccount.OwnerId);
            return View(bankAccount);
        }

        // GET: BankAccounts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts
                .Include(b => b.Owner)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (bankAccount == null)
            {
                return NotFound();
            }

            return View(bankAccount);
        }

        // POST: BankAccounts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            _context.BankAccounts.Remove(bankAccount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool BankAccountExists(int id)
        {
            return _context.BankAccounts.Any(e => e.Id == id);
        }

        public async Task<IActionResult> HandleDeposit(int id, int amount)
        {
            if (!BankAccountExists(id))
            {
                return NotFound();
            }
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            bankAccount.MakeDeposit(amount);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> HandleWithdrawal(int id, int amount)
        {
            if (!BankAccountExists(id))
            {
                return NotFound();
            }
            var bankAccount = await _context.BankAccounts.FindAsync(id);
            bankAccount.MakeWithdrawal(amount);
            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }

        public async Task<IActionResult> HandleTransfer(int id, int amount, int otherAccountId)
        {
            if (!BankAccountExists(id))
            {
                return NotFound();
            }

            var bankAccount = await _context.BankAccounts.FindAsync(id);
            if (!BankAccountExists(otherAccountId))
            {
                bankAccount.AddEntry(amount, EventType.Failed, $"Transfer {EventType.Failed.ToString()}: account #{otherAccountId} does not exist.");
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id });
            }

            var otherAccount = await _context.BankAccounts.FindAsync(otherAccountId);
            bankAccount.TransferMoney(otherAccount, amount);

            await _context.SaveChangesAsync();
            return RedirectToAction("Details", new { id });
        }
    }
}
