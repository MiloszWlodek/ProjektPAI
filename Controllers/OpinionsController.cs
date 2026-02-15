using InfoInfo.Data;
using InfoInfo.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace InfoInfo.Controllers
{
    public class OpinionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public OpinionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Opinions (Dla Administratora)
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Index(int? id)
        {
            if (id == null)
            {
                var allOpinions = _context.Opinions.Include(o => o.Author).Include(o => o.Text);
                return View(await allOpinions.ToListAsync());
            }
            else
            {
                var filteredOpinions = _context.Opinions.Include(o => o.Author).Include(o => o.Text)
                    .Where(o => o.TextId == id);
                return View(await filteredOpinions.ToListAsync());
            }
        }

        // GET: Opinions/Create
        [Authorize]
        public IActionResult Create(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Text? text = _context.Texts.Find(id);
            if (text == null)
            {
                return BadRequest();
            }

            var opinion = new Opinion
            {
                UserId = User.FindFirstValue(ClaimTypes.NameIdentifier),
                TextId = text.TextId
            };

            ViewData["TextTitle"] = text.Title;
            return View(opinion);
        }

        // POST: Opinions/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Create([Bind("OpinionId,Comment,Rating,TextId,UserId")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                opinion.AddedDate = DateTime.Now;
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Texts", new { id = opinion.TextId }, "comments");
            }
            opinion.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["TextTitle"] = _context.Texts.Find(opinion.TextId)?.Title;
            return View(opinion);
        }

        // POST: Opinions/CreatePartial
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> CreatePartial([Bind("OpinionId,Comment,Rating,TextId,UserId")] Opinion opinion)
        {
            if (ModelState.IsValid)
            {
                opinion.AddedDate = DateTime.Now;
                _context.Add(opinion);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", "Texts", new { id = opinion.TextId }, "comments");
            }
            opinion.UserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            ViewData["TextTitle"] = opinion.Text?.Title;
            return RedirectToAction("Details", "Texts", new { id = opinion.TextId }, "comments");
        }

        // Akcje Edit i Delete można wygenerować standardowo (scaffold), poniżej przykładowe Delete
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var opinion = await _context.Opinions.Include(o => o.Author).Include(o => o.Text).FirstOrDefaultAsync(m => m.OpinionId == id);
            if (opinion == null) return NotFound();
            return View(opinion);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var opinion = await _context.Opinions.FindAsync(id);
            if (opinion != null) _context.Opinions.Remove(opinion);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}