using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using TaskListLab.Models;

namespace TaskListLab.Controllers
{
    [Authorize]
    public class TaskListController : Controller
    {
        private readonly TaskListDBContext _context;
        public TaskListController(TaskListDBContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<TaskList> userTaskList = _context.TaskList.Where(x => x.UserId == id).ToList();

            // reset the User ID to be the User Name which displays nicely on the UI
            userTaskList.ForEach(x => x.UserId = User.FindFirst(ClaimTypes.Name).Value);

            return View(userTaskList);
        }

        [HttpGet]
        public IActionResult CreateTask()
        {
            return View();
        }

        [HttpPost]
        public IActionResult CreateTask(TaskList task)
        {
            task.UserId = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ModelState.IsValid)
            {
                _context.TaskList.Add(task);
                _context.SaveChanges();
                return RedirectToAction("Index");
            } else
            {
                return RedirectToAction("CreateTask");
            }
        }

        private TaskList getTaskList(int taskId)
        {
            string id = User.FindFirst(ClaimTypes.NameIdentifier).Value;
            List<TaskList> userTaskList = _context.TaskList.Where(x => x.UserId == id && x.Id == taskId).ToList();

            if (userTaskList.Count() == 1)
            {
                return userTaskList[0];
            } else
            {
                // if more than 1 came back, we don't send anything back
                // this means delete and mark complete won't work and
                // the user will complain and a dev can investigate why this ID is not unique
                return null;
            }
        }
        public IActionResult ToggleComplete(int taskId)
        {
            TaskList thisTask = getTaskList(taskId);

            if (thisTask != null)
            {
                thisTask.Complete = !thisTask.Complete;// toggle the task completion
                _context.Entry(thisTask).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
                _context.Update(thisTask);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        public IActionResult DeleteTask(int taskId)
        {
            TaskList thisTask = getTaskList(taskId);

            if (thisTask != null)
            {
                _context.TaskList.Remove(thisTask);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}