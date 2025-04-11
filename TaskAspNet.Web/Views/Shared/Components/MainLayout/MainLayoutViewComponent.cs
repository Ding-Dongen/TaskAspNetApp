using Microsoft.AspNetCore.Mvc;

public class MainLayoutViewComponent : ViewComponent
{
    public IViewComponentResult Invoke()
    {
        return View();
    }
}
