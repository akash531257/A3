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
    public class a1PlotController : Controller
    {
        private readonly OECContext _context;

        public a1PlotController(OECContext context)
        {
            _context = context;
        }

        // GET: a1Plot
        public async Task<IActionResult> Index(int? id,String searchcriteria, String varietyname)
        {
            var oECContext = _context.Plot;
            

            if(id!=null && searchcriteria!=null)
            {
                HttpContext.Session.SetInt32("a_id", Convert.ToInt32(id));
                HttpContext.Session.SetString("a_sc", searchcriteria);
                HttpContext.Session.SetString("a_varietyname", varietyname);

            }
            else if(id==null && searchcriteria!=null)
            {
                searchcriteria = "plot";
                HttpContext.Session.SetInt32("a_id", Convert.ToInt32(id));
                HttpContext.Session.SetString("a_sc", searchcriteria);
               
                

            }
            else if(id!=null && searchcriteria==null)
            {
                searchcriteria = "specificplot";
                HttpContext.Session.SetInt32("a_id", Convert.ToInt32(id));
               
            }


            switch (searchcriteria)
            {
                case "crop":

                    return View(await _context.Plot
                 .Include(p => p.Farm)
                 .Include(p => p.Variety)
                 .Include(p => p.Treatment)
                 .Include(p => p.Variety.Crop)
                 .Where(p => p.Variety.Crop.CropId == id).ToListAsync());

                case "variety":
                    return View(await _context.Plot
                                       .Include(p => p.Farm)
                                       .Include(p => p.Variety)
                                       .Include(p => p.Treatment)
                                      .Include(p => p.Variety.Crop)
                                      .Where(p => p.Variety.VarietyId == id).ToListAsync());
                case "specificplot":
                    return View(await _context.Plot
            .Include(p => p.Farm)
             .Include(p => p.Variety)
             .Include(p => p.Treatment)
             .Where(p =>p.PlotId==id)
             .OrderBy(a => a.Farm.Name).ToListAsync());

                default:
                    return View(await _context.Plot
               .Include(p => p.Farm)
                .Include(p => p.Variety)
                .Include(p => p.Treatment)
                .OrderBy(a => a.Farm.Name).ToListAsync());

            }

            //var oECContext = _context.Plot
            //   .Include(p => p.Farm)
            //    .Include(p => p.Variety)
            //    .Include(p=>p.Treatment)
            //    .OrderBy(a=>a.Farm.Name);
            // collect treatment names, sorted, into comments
            //foreach (var item in oECContext)
            //{
            //    item.Comments = ""; // only string available
            //    item.Treatment.OrderBy(a => a.Name);
            //    foreach (var treatment in item.Treatment)
            //    {
            //        item.Comments = item.Comments + " <br /> " + treatment.Name;
            //    }
            //    if (item.Comments.Length > 6) item.Comments = item.Comments.Substring(6);
            //}
            //return View(await oECContext.ToListAsync());
        }

        // GET: a1Plot/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // GET: a1Plot/Create
        public IActionResult Create()
        {
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode");
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId");
            return View();
        }

        // POST: a1Plot/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (ModelState.IsValid)
            {
                _context.Add(plot);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: a1Plot/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // POST: a1Plot/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlotId,FarmId,VarietyId,DatePlanted,DateHarvested,PlantingRate,PlantingRateByPounds,RowWidth,PatternRepeats,OrganicMatter,BicarbP,Potassium,Magnesium,Calcium,PHsoil,PHbuffer,Cec,PercentBaseSaturationK,PercentBaseSaturationMg,PercentBaseSaturationCa,PercentBaseSaturationH,Comments")] Plot plot)
        {
            if (id != plot.PlotId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plot);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlotExists(plot.PlotId))
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
            ViewData["FarmId"] = new SelectList(_context.Farm, "FarmId", "ProvinceCode", plot.FarmId);
            ViewData["VarietyId"] = new SelectList(_context.Variety, "VarietyId", "VarietyId", plot.VarietyId);
            return View(plot);
        }

        // GET: a1Plot/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var plot = await _context.Plot
                .Include(p => p.Farm)
                .Include(p => p.Variety)
                .SingleOrDefaultAsync(m => m.PlotId == id);
            if (plot == null)
            {
                return NotFound();
            }

            return View(plot);
        }

        // POST: a1Plot/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var plot = await _context.Plot.SingleOrDefaultAsync(m => m.PlotId == id);
            _context.Plot.Remove(plot);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlotExists(int id)
        {
            return _context.Plot.Any(e => e.PlotId == id);
        }
    }
}
