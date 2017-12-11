using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using a2OEC.Models;
using Microsoft.AspNetCore.Http;

namespace a2OEC.Controllers
{
    public class APTreatmentFertilizerController : Controller
    {
        private readonly OECContext _context;

        public APTreatmentFertilizerController(OECContext context)
        {
            _context = context;
        }

        // GET: APTreatmentFertilizer
        public async Task<IActionResult> Index(int? id)
        {
            if (id != null)
            {
                HttpContext.Session.SetInt32("treatmentid", Convert.ToInt32(id));
            }
            else if (id == null && HttpContext.Session.GetInt32("treatmentid") == null)
            {
                TempData["message"] = "Select a Treatment to see it's fertilizer composition";
                return RedirectToAction("Index", "APTreatment");
            }
            else
            {
                id = Convert.ToInt32(HttpContext.Session.GetInt32("treatmentid"));
            }


            var oECContext = _context.TreatmentFertilizer
                            .Include(t => t.FertilizerNameNavigation)
                            .Include(t => t.Treatment)
                            .Where(t =>t.TreatmentId == id)
                            .OrderBy(t =>t.FertilizerName);

            ViewData["plotid"] = HttpContext.Session.GetInt32("plotid");
            ViewData["farmname"] = HttpContext.Session.GetString("farmname");
            return View(await oECContext.ToListAsync());
        }

        // GET: APTreatmentFertilizer/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            return View(treatmentFertilizer);
        }

        // GET: APTreatmentFertilizer/Create
        public IActionResult Create()
        {   
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy( t =>t.FertilizerName), "FertilizerName", "FertilizerName");
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId");
            return View();
        }

        // POST: APTreatmentFertilizer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            if (ModelState.IsValid)
            {
               treatmentFertilizer.TreatmentId= Convert.ToInt32(HttpContext.Session.GetInt32("treatmentid"));
                _context.Add(treatmentFertilizer);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer, "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);
            return View(treatmentFertilizer);
        }

        // GET: APTreatmentFertilizer/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer.OrderBy(t =>t.FertilizerName), "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);
            return View(treatmentFertilizer);
        }

        // POST: APTreatmentFertilizer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentFertilizerId,TreatmentId,FertilizerName,RatePerAcre,RateMetric")] TreatmentFertilizer treatmentFertilizer)
        {
            if (id != treatmentFertilizer.TreatmentFertilizerId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    treatmentFertilizer.TreatmentId = Convert.ToInt32(HttpContext.Session.GetInt32("treatmentid"));
                    _context.Update(treatmentFertilizer);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentFertilizerExists(treatmentFertilizer.TreatmentFertilizerId))
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
            ViewData["FertilizerName"] = new SelectList(_context.Fertilizer, "FertilizerName", "FertilizerName", treatmentFertilizer.FertilizerName);
            ViewData["TreatmentId"] = new SelectList(_context.Treatment, "TreatmentId", "TreatmentId", treatmentFertilizer.TreatmentId);
            return View(treatmentFertilizer);
        }

        // GET: APTreatmentFertilizer/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var treatmentFertilizer = await _context.TreatmentFertilizer
                .Include(t => t.FertilizerNameNavigation)
                .Include(t => t.Treatment)
                .SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            if (treatmentFertilizer == null)
            {
                return NotFound();
            }

            return View(treatmentFertilizer);
        }

        // POST: APTreatmentFertilizer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var treatmentFertilizer = await _context.TreatmentFertilizer.SingleOrDefaultAsync(m => m.TreatmentFertilizerId == id);
            treatmentFertilizer.TreatmentId = Convert.ToInt32(HttpContext.Session.GetInt32("treatmentid"));
            _context.TreatmentFertilizer.Remove(treatmentFertilizer);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentFertilizerExists(int id)
        {
            return _context.TreatmentFertilizer.Any(e => e.TreatmentFertilizerId == id);
        }
    }
}
