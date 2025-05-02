using Models.ViewModels;

namespace Services.WebshopConfigServices.Interfaces
{
    public interface IWebshopSettingsServices
    {
        Task<WebshopSettingsVm> GetVmAsync();
        Task UpdateAsync(WebshopSettingsVm vm);
    }
}