using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SaudePedraBela.Data;
using SaudePedraBela.Models;
using SaudePedraBela.Filters;


namespace SaudePedraBela.Controllers


{
    [LoginFilter]
    public class DocumentosController : Controller
    {
        private readonly SaudePedraBelaContext _context;

        public DocumentosController(SaudePedraBelaContext context)
        {
            _context = context;
        }
       

        // GET: Documentos
        public async Task<IActionResult> Index()
        {
            var documentos = _context.Documento.Include(d => d.Categorias);
            return View(await documentos.ToListAsync());
        }

        // GET: Documentos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        // GET: Documentos/Create
        public IActionResult Create()
        {
            ViewData["IdCategoria"] = new SelectList(_context.Categorias, "Id", "NomeCategoria");
            return View();
        }

        // POST: Documentos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Documento documento, IFormFile arquivoPdf)
        {
            if (arquivoPdf != null && arquivoPdf.Length > 0)
            {
                var nomeArquivo = Path.GetFileName(arquivoPdf.FileName);

                var caminho = Path.Combine(Directory.GetCurrentDirectory(),
                "wwwroot/documentos", nomeArquivo);

                using (var stream = new FileStream(caminho, FileMode.Create))
                {
                    await arquivoPdf.CopyToAsync(stream);
                }

                documento.CaminhoDocumento = "/documentos/" + nomeArquivo;
            }

            _context.Add(documento);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // GET: Documentos/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documento.FindAsync(id);
            if (documento == null)
            {
                return NotFound();
            }
            return View(documento);
        }

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ListaPublica()
        {
            var categorias = await _context.Categorias
                .Include(c => c.Documentos)
                .Select(c => new
                {
                    nomeCategoria = c.NomeCategoria,
                    documentos = c.Documentos.Select(d => new
                    {
                        nomeDocumento = d.NomeDocumento,
                        caminhoDocumento = d.CaminhoDocumento
                    })
                })
                .ToListAsync();

            return Json(categorias);
        }

        // POST: Documentos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NomeDocumento,CaminhoDocumento,IdCategoria")] Documento documento)
        {
            if (id != documento.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(documento);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DocumentoExists(documento.Id))
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
            return View(documento);
        }

        // GET: Documentos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var documento = await _context.Documento
                .FirstOrDefaultAsync(m => m.Id == id);
            if (documento == null)
            {
                return NotFound();
            }

            return View(documento);
        }

        // POST: Documentos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var documento = await _context.Documento.FindAsync(id);
            if (documento != null)
            {
                _context.Documento.Remove(documento);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DocumentoExists(int id)
        {
            return _context.Documento.Any(e => e.Id == id);
        }
    }
}
