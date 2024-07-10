using MudBlazor;
using MudBlazor.Utilities;

namespace DutyDock.Web;

public partial class App
{
    private static readonly MudTheme Theme = new()
    {
        PaletteLight =
        {
            // Primary = new MudColor(IzColors.Primary),
            // Secondary = new MudColor(IzColors.Secondary),
            // Tertiary = new MudColor(IzColors.Blue300),
            // Info = new MudColor(IzColors.Blue200),
            // Success = new MudColor(IzColors.Success),
            // Warning = new MudColor(IzColors.Warning),
            // Error = new MudColor(IzColors.Error)
        },
        Typography = new Typography
        {
            Default = new Default
            {
                FontFamily = new[]
                {
                    "Albert Sans",
                    "sans-serif"
                }
            }
        }
    };
}