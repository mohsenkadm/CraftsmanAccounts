// متحكم أنواع الاشتراكات - إنشاء وتعديل وحذف أنواع الاشتراكات
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using CraftsmanAccounts.Web.Models;
using CraftsmanAccounts.Web.Services.Interfaces;

namespace CraftsmanAccounts.Web.Controllers;

[Authorize]
public class SubscriptionTypesController : Controller
{
    private readonly ISubscriptionTypeService _service;

    public SubscriptionTypesController(ISubscriptionTypeService service)
    {
        _service = service;
    }

    public IActionResult Index()
    {
        var subscriptions = _service.GetAll();
        return View(subscriptions);
    }

    [HttpGet]
    public IActionResult Create() => View(new SubscriptionType());

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Create(SubscriptionType model)
    {
        if (!ModelState.IsValid) return View(model);

        _service.Create(model);
        TempData["Success"] = "تم إضافة نوع الاشتراك بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public IActionResult Edit(int id)
    {
        var subscription = _service.GetById(id);
        if (subscription is null) return NotFound();
        return View(subscription);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Edit(SubscriptionType model)
    {
        if (!ModelState.IsValid) return View(model);

        _service.Update(model);
        TempData["Success"] = "تم تحديث نوع الاشتراك بنجاح";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult Delete(int id)
    {
        _service.Delete(id);
        TempData["Success"] = "تم حذف نوع الاشتراك بنجاح";
        return RedirectToAction(nameof(Index));
    }
}
