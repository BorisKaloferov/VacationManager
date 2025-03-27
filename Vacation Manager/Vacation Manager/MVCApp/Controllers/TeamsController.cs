using System;
using System.Linq;
using System.Threading.Tasks;
using Business_Layer;
using Data_Layer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace MVCApplication.Controllers
{
    [Authorize]
    public class TeamsController : Controller
    {
        private readonly TeamContext _teamContext;
        private readonly ProjectContext _projectContext;
        private readonly IdentityContext _identityContext;

        public TeamsController(TeamContext teamContext, ProjectContext projectContext, IdentityContext identityContext)
        {
            _teamContext = teamContext;
            _projectContext = projectContext;
            _identityContext = identityContext;
        }

        // GET: Teams
        public async Task<IActionResult> Index()
        {
            var teams = await _teamContext.ReadAllAsync(useNavigationalProperties: true);
            return View(teams);
        }

        // GET: Teams/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await _teamContext.ReadAsync((int)id, useNavigationalProperties: true);
            if (team == null)
                return NotFound();

            return View(team);
        }

        // GET: Teams/Create
        public async Task<IActionResult> Create()
        {
            await LoadNavigationalPropertiesAsync();
            return View();
        }

        // POST: Teams/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,ProjectId,LeaderId")] Team team)
        {
            if (ModelState.IsValid)
            {
                // Load and attach the related Project and Leader using the manager classes.
                team.Project = await _projectContext.ReadAsync(team.ProjectId, useNavigationalProperties: false);
                team.Leader = await _identityContext.ReadAsync(team.LeaderId, useNavigationalProperties: false);

                await _teamContext.CreateAsync(team);
                return RedirectToAction(nameof(Index));
            }

            await LoadNavigationalPropertiesAsync();
            return View(team);
        }

        // GET: Teams/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await _teamContext.ReadAsync((int)id, useNavigationalProperties: true);
            if (team == null)
                return NotFound();

            await LoadNavigationalPropertiesAsync();
            return View(team);
        }

        // POST: Teams/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,ProjectId,LeaderId")] Team team)
        {
            if (id != team.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing team including its navigational properties.
                    var teamToUpdate = await _teamContext.ReadAsync(id, useNavigationalProperties: true, isReadOnly: false);
                    if (teamToUpdate == null)
                        return NotFound();

                    // Update scalar properties.
                    teamToUpdate.Name = team.Name;
                    // Update related entities.
                    teamToUpdate.Project = await _projectContext.ReadAsync(team.ProjectId, useNavigationalProperties: false);
                    teamToUpdate.Leader = await _identityContext.ReadAsync(team.LeaderId, useNavigationalProperties: false);

                    await _teamContext.UpdateAsync(teamToUpdate, useNavigationalProperties: true);
                }
                catch (Exception)
                {
                    if (!await TeamExists(team.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }

            await LoadNavigationalPropertiesAsync();
            return View(team);
        }

        // GET: Teams/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var team = await _teamContext.ReadAsync((int)id, useNavigationalProperties: true);
            if (team == null)
                return NotFound();

            return View(team);
        }

        // POST: Teams/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _teamContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> TeamExists(int id)
        {
            return (await _teamContext.ReadAsync(id)) != null;
        }

        /// <summary>
        /// Loads available Projects and Users into ViewData for dropdown lists in Create and Edit views.
        /// </summary>
        private async Task LoadNavigationalPropertiesAsync()
        {
            var projects = await _projectContext.ReadAllAsync(useNavigationalProperties: false);
            var users = await _identityContext.ReadAllAsync(useNavigationalProperties: false);
            ViewData["ProjectId"] = new SelectList(projects, "Id", "Name");
            ViewData["LeaderId"] = new SelectList(users, "Id", "Name");
        }
    }
}
