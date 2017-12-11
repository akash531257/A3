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
    public class APTreatmentController : Controller
    {
        private readonly OECContext _context;

        public APTreatmentController(OECContext context)
        {
            _context = context;
        }

        // GET: APTreatment
        public async Task<IActionResult> Index(int? plotid ,String farmname)
        {
            if (plotid != null)
            {
                
                HttpContext.Session.SetInt32("plotid", Convert.ToInt32(plotid));
                HttpContext.Session.SetString("farmname", farmname);
            }
            else if (plotid == null && HttpContext.Session.GetInt32("plotid") == null)
            {
                TempData["message"] = "Select a Plot to see it's treatments";
                return RedirectToAction("Index", "a1Plot");
            }
            else
            {
                plotid = Convert.ToInt32(HttpContext.Session.GetInt32("plotid"));
            }

            var oECContext = _context.Treatment.Include(t => t.Plot)
                            .Include(t => t.Plot.Farm)
                           .Include(t =>t.TreatmentFertilizer)
                            .Where(t=>t.PlotId==plotid)
                            .OrderBy(t=>t.Name);

            ViewData["cropName"] = HttpContext.Session.GetString("a_cropname");
            ViewData["varietyName"] = HttpContext.Session.GetString("varietyName");
            ViewData["farmname"] = farmname;
            ViewData["plotId"] = plotid;
         //   HttpContext.Session.SetString("a_farmname", farmname);
            return View(await oECContext.ToListAsync());

        }
       

        // GET: APTreatment/Details/5
        public async Task<IActionResult> Details(int? id ,String farmname)
        {
            ViewData["farmname"] = farmname;
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }

            return View(treatment);
        }

        // GET: APTreatment/Create
        public IActionResult Create(int? id, String farmname)
        {
            ViewData["farmname"] = farmname;
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId");
            return View();
        }

        // POST: APTreatment/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            
            if (ModelState.IsValid)
            {
                treatment.PlotId =Convert.ToInt32(HttpContext.Session.GetInt32("plotid"));
                _context.Add(treatment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PlotId"] = treatment.PlotId;
            //ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // GET: APTreatment/Edit/5
        public async Task<IActionResult> Edit(int? id,String farmname)
        {
            ViewData["farmname"] = farmname;

            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            
            if (treatment == null)
            {
                return NotFound();
            }
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // POST: APTreatment/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TreatmentId,Name,PlotId,Moisture,Yield,Weight")] Treatment treatment)
        {
            if (id != treatment.TreatmentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    treatment.PlotId = Convert.ToInt32(HttpContext.Session.GetInt32("plotid"));
                    _context.Update(treatment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TreatmentExists(treatment.TreatmentId))
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
            ViewData["PlotId"] = new SelectList(_context.Plot, "PlotId", "PlotId", treatment.PlotId);
            return View(treatment);
        }

        // GET: APTreatment/Delete/5
        public async Task<IActionResult> Delete(int? id, String farmname)
        {
            ViewData["farmname"] = farmname;
            if (id == null)
            {
                return NotFound();
            }

            var treatment = await _context.Treatment
                .Include(t => t.Plot)
                .SingleOrDefaultAsync(m => m.TreatmentId == id);
            if (treatment == null)
            {
                return NotFound();
            }
            

            return View(treatment);
        }

        // POST: APTreatment/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
          
            var treatment = await _context.Treatment.SingleOrDefaultAsync(m => m.TreatmentId == id);
            treatment.PlotId = Convert.ToInt32(HttpContext.Session.GetInt32("plotid"));
            _context.Treatment.Remove(treatment);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TreatmentExists(int id)
        {
            return _context.Treatment.Any(e => e.TreatmentId == id);
        }
    }
}
