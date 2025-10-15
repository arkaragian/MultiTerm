// using Terminal.Gui;
//
// namespace MultiTermCLI.Tui;
//
// public class MultiTermMenu : MenuBarv2 {
//     public MultiTermMenu() {
//
//         X = 0;
//         Y = 0;
//         Width = Dim.Fill();
//         Height = 1;
//
//         // File menu
//         MenuBarItemv2 fileRoot = new MenuBarItemv2() {
//             Text = "_File"
//         };
//
//         PopoverMenu fileMenu = new PopoverMenu();
//         fileMenu.Add(new MenuItemv2() {
//             Text = "_Open",
//             HelpText = "Open a file",
//             Key = Key.CtrlMask | Key.O,
//             Action = () => {
//                 MessageBox.Query(40, 7, "Open", "Open clicked", "OK");
//             }
//         });
//         fileMenu.Add(new MenuItemv2() {
//             Text = "_Quit",
//             HelpText = "Exit",
//             Key = Key.CtrlMask | Key.Q,
//             Action = () => {
//                 Application.RequestStop();
//             }
//         });
//         fileRoot.PopoverMenu = fileMenu;
//
//         // Help menu
//         MenuBarItemv2 helpRoot = new MenuBarItemv2() {
//             Text = "_Help"
//         };
//         PopoverMenu helpMenu = new PopoverMenu();
//         helpMenu.Add(new MenuItemv2() {
//             Text = "_About",
//             HelpText = "About this app",
//             Key = Key.F1,
//             Action = () => {
//                 MessageBox.Query(40, 7, "About", "Terminal.Gui v2 demo", "OK");
//             }
//         });
//         helpRoot.PopoverMenu = helpMenu;
//
//         // Attach items to the bar
//         Add(fileRoot, helpRoot);
//     }
// }
//