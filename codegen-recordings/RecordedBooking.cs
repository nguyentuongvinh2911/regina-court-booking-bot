using Microsoft.Playwright.NUnit;
using Microsoft.Playwright;

[Parallelizable(ParallelScope.Self)]
[TestFixture]
public class Tests : PageTest
{
    [Test]
    public async Task MyTest()
    {
        await Page.GotoAsync("https://anc.ca.apm.activecommunities.com/regina/reservation/landing");
        await Page.GetByRole(AriaRole.Link, new() { Name = "Badminton Bookings" }).ClickAsync();
        await Page.GetByRole(AriaRole.Link, new() { Name = "Sign in now" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Login name Required" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Login name Required" }).FillAsync("<LOGIN_NAME>");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Login name Required" }).PressAsync("Tab");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password Required" }).FillAsync("<PASSWORD>");
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Password Required" }).PressAsync("Enter");
        await Page.GetByRole(AriaRole.Combobox, new() { Name = "Date picker, current date" }).ClickAsync();
        await Page.GetByRole(AriaRole.Region, new() { Name = "Mar 21," }).ClickAsync();
        await Page.GetByRole(AriaRole.Gridcell, new() { Name = "FH - Badminton Court 1 8:00 PM - 9:00 PM Available" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Event name" }).ClickAsync();
        await Page.GetByRole(AriaRole.Textbox, new() { Name = "Event name" }).FillAsync("1");
        await Page.GetByRole(AriaRole.Button, new() { Name = "1 Resource(s), 1 Booking(s)" }).ClickAsync();
        await Page.GetByRole(AriaRole.Group, new() { Name = "*What is Player 2's full name" }).GetByLabel("Input box").ClickAsync();
        await Page.GetByRole(AriaRole.Group, new() { Name = "*What is Player 2's full name" }).GetByLabel("Input box").FillAsync("<PLAYER_2_FULL_NAME>");
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I have read and agree to Court User Guidelines" }).CheckAsync();
        await Page.GetByRole(AriaRole.Checkbox, new() { Name = "I have read and agree to Non-" }).CheckAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Save" }).ClickAsync();
        await Page.GetByRole(AriaRole.Button, new() { Name = "Total $0.00 Reserve" }).ClickAsync();
    }
}
