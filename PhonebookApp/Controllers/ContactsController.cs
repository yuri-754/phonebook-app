using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PhonebookApp.Areas.Identity.Data;
using PhonebookApp.Data;

namespace PhonebookApp.Controllers
{
    public class ContactsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<PhonebookUser> _userManager;

        public ContactsController(ApplicationDbContext context, UserManager<PhonebookUser> userManager)
        {
            _context = context;
            this._userManager = userManager;
        }
        //GET: Logged User
        public async Task<PhonebookUser> GetUser()
        {
            var user = await _userManager.GetUserAsync(this.User);
            return user;
        }

        // GET: Contacts
        public async Task<IActionResult> Index()
        {
            //check if logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await GetUser();
            
            var applicationDbContext = _context.Contacts
                .Where(c => c.UserId == user.Id)
                .Include(c => c.User);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Contacts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            //check if logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await GetUser();
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }
            


            var contact = await _context.Contacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            if (contact.UserId != user.Id)
            {
                return NotFound();
            }

            return View(contact);
        }

        // GET: Contacts/Create
        public async Task<IActionResult> Create()
        {
            //check if logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id");
            var user = await GetUser();
            ViewBag.UserId = user.Id;
            return View();
        }

        // POST: Contacts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,PhoneNumber,Email,UserId")] Contact contact)
        {
            if (PhonenNumberExists(contact.PhoneNumber))
            {
                ViewBag.Error = "Phone number exists";
            }
            else
            {
                if (ModelState.IsValid)
                {
                    _context.Add(contact);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contact.UserId);
            return View(contact);
        }

        // GET: Contacts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            //check if logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await GetUser();

            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts.FindAsync(id);
            if (contact == null)
            {
                return NotFound();
            }
            if(contact.UserId != user.Id)
            {
                return NotFound();
            }


            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contact.UserId);
            return View(contact);
        }

        // POST: Contacts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,PhoneNumber,Email,UserId")] Contact contact)
        {
            var user = await GetUser();

            if (id != contact.Id)
            {
                return NotFound();
            }
            if (contact.UserId != user.Id)
            {
                return NotFound();
            }
            //get old phone number and compare it to other phone number but not with itself
            var oldPhoneNumber = await _context.Contacts
                .Where(c => c.Id == contact.Id)
                .Select(c => c.PhoneNumber)
                .FirstOrDefaultAsync();
            if (PhonenNumberExists(contact.PhoneNumber) && contact.PhoneNumber != oldPhoneNumber)
            {
                ViewBag.Error = "Phone number exists";
            }                   
            else
            {
                if (ModelState.IsValid)
                {
                    try
                    {
                        _context.Update(contact);
                        await _context.SaveChangesAsync();
                    }
                    catch (DbUpdateConcurrencyException)
                    {
                        if (!ContactExists(contact.Id))
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
            }
            //ViewData["UserId"] = new SelectList(_context.Users, "Id", "Id", contact.UserId);
            return View(contact);
        }

        // GET: Contacts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            //check if logged in
            if (!User.Identity.IsAuthenticated)
            {
                return Unauthorized();
            }
            var user = await GetUser();
            if (id == null || _context.Contacts == null)
            {
                return NotFound();
            }

            var contact = await _context.Contacts
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (contact == null)
            {
                return NotFound();
            }
            if (contact.UserId != user.Id)
            {
                return NotFound();
            }

            return View(contact);
        }

        // POST: Contacts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Contacts == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Contacts'  is null.");
            }
            var contact = await _context.Contacts.FindAsync(id);
            if (contact != null)
            {
                _context.Contacts.Remove(contact);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ContactExists(int id)
        {
          return (_context.Contacts?.Any(e => e.Id == id)).GetValueOrDefault();
        }
        private bool PhonenNumberExists(string phoneNumber)
        {
            return (_context.Contacts?.Any(e => e.PhoneNumber == phoneNumber)).GetValueOrDefault();
        }
    }
}
