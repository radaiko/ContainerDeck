using ContainerDeck.Shared.Models;
using ContainerDeck.Shared.Services;
using Microsoft.AspNetCore.Components;

namespace ContainerDeck.Shared.Components {
    public partial class AgentComponent : ComponentBase {
        [Inject]
        private AgentsService AgentsService { get; set; } = null!;

        [Parameter]
        public Agent This { get; set; } = null!;

        private string _agentVersion = "Unknown";
        private string _dockerVersion = "Unknown";
        private bool _isHealthy = false;
        private bool dialogShown = false;

        protected override async Task OnInitializedAsync() {
            _agentVersion = await This.AgentVersion();
            _dockerVersion = await This.DockerVersion();
            _isHealthy = await This.IsHealthy();
        }

        private void EditAgent() {
            dialogShown = true;
        }

        private void SaveAgent() {
            AgentsService.Delete(This);
            AgentsService.Add(This);
            dialogShown = false;
        }

        private void DeleteAgent() {
            AgentsService.Delete(This);
            StateHasChanged();
        }

        private void CancelEdit() {
            dialogShown = false;
        }
    }
}