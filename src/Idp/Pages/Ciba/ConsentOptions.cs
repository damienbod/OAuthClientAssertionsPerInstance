// Copyright (c) Duende Software. All rights reserved.
// See LICENSE in the project root for license information.


namespace Idp.Pages.Ciba;

public class ConsentOptions
{
    public static bool EnableOfflineAccess = true;
    public static string OfflineAccessDisplayName = "Offline Access";
    public static string OfflineAccessDescription = "Access to your applications and resources, even when you are offline";

    public static readonly string MustChooseOneErrorMessage = "You must pick at least one permission";
    public static readonly string InvalidSelectionErrorMessage = "Invalid selection";
}