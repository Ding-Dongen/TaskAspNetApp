
using TaskAspNet.Business.Dtos;

namespace TaskAspNet.Business.ViewModel;

public class ClientIndexViewModel
{
    public List<ClientDto> AllMembers { get; set; } = new();
    public ClientDto CreateClient { get; set; } = new();

}
