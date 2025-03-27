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
    public class ProjectsController : Controller
    {
        private readonly ProjectContext _projectContext;
        private readonly TeamContext _teamContext;

        public ProjectsController(ProjectContext projectContext, TeamContext teamContext)
        {
            _projectContext = projectContext;
            _teamContext = teamContext;
        }

        // GET: Projects
        public async Task<IActionResult> Index()
        {
            var projects = await _projectContext.ReadAllAsync(useNavigationalProperties: true);
            return View(projects);
        }

        // GET: Projects/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _projectContext.ReadAsync((int)id, useNavigationalProperties: true);
            if (project == null)
                return NotFound();

            return View(project);
        }

        // GET: Projects/Create
        public async Task<IActionResult> Create()
        {
            await LoadNavigationalPropertiesAsync();
            return View();
        }

        // POST: Projects/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Description")] Project project, int[] selectedTeamIds)
        {
            // Clear and update the model (if needed)
            ModelState.Clear();
            await TryUpdateModelAsync(project);

            if (ModelState.IsValid)
            {
                // Attach selected teams using the TeamContext
                if (selectedTeamIds != null && selectedTeamIds.Any())
                {
                    foreach (var teamId in selectedTeamIds)
                    {
                        Team team = await _teamContext.ReadAsync(teamId, useNavigationalProperties: false);
                        if (team != null)
                        {
                            project.Teams.Add(team);
                        }
                    }
                }
                await _projectContext.CreateAsync(project);
                return RedirectToAction(nameof(Index));
            }

            await LoadNavigationalPropertiesAsync();
            return View(project);
        }

        // GET: Projects/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _projectContext.ReadAsync((int)id, useNavigationalProperties: true);
            if (project == null)
                return NotFound();

            await LoadNavigationalPropertiesAsync(project);
            return View(project);
        }

        // POST: Projects/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description")] Project project, int[] selectedTeamIds)
        {
            if (id != project.Id)
                return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // Retrieve the existing project with navigation properties.
                    var projectToUpdate = await _projectContext.ReadAsync(id, useNavigationalProperties: true, isReadOnly: false);
                    if (projectToUpdate == null)
                        return NotFound();

                    // Update scalar properties.
                    projectToUpdate.Name = project.Name;
                    projectToUpdate.Description = project.Description;

                    // Update teams: clear current associations and add the selected teams.
                    projectToUpdate.Teams.Clear();
                    if (selectedTeamIds != null && selectedTeamIds.Any())
                    {
                        foreach (var teamId in selectedTeamIds)
                        {
                            Team team = await _teamContext.ReadAsync(teamId, useNavigationalProperties: false);
                            if (team != null)
                            {
                                projectToUpdate.Teams.Add(team);
                            }
                        }
                    }

                    await _projectContext.UpdateAsync(projectToUpdate, useNavigationalProperties: true);
                }
                catch (Exception)
                {
                    if (!await ProjectExists(project.Id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            await LoadNavigationalPropertiesAsync(project);
            return View(project);
        }

        // GET: Projects/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var project = await _projectContext.ReadAsync((int)id, useNavigationalProperties: true);
            if (project == null)
                return NotFound();

            return View(project);
        }

        // POST: Projects/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _projectContext.DeleteAsync(id);
            return RedirectToAction(nameof(Index));
        }

        private async Task<bool> ProjectExists(int id)
        {
            return (await _projectContext.ReadAsync(id)) != null;
        }

        /// <summary>
        /// Loads teams into ViewData for use in the Create and Edit views.
        /// </summary>
        /// <param name="project">An optional project to mark already-selected teams.</param>
        private async Task LoadNavigationalPropertiesAsync(Project project = null)
        {
            var teams = await _teamContext.ReadAllAsync(useNavigationalProperties: false);
            if (project != null)
            {
                var selectedTeamIds = project.Teams.Select(t => t.Id).ToList();
                ViewData["Teams"] = new MultiSelectList(teams, "Id", "Name", selectedTeamIds);
            }
            else
            {
                ViewData["Teams"] = new MultiSelectList(teams, "Id", "Name");
            }
        }
    }
}
