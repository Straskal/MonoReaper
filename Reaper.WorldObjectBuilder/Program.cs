using Reaper.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using Terminal.Gui;

namespace Reaper.WorldObjectBuilder
{
    class Program
    {
        static WorldObject currentWO;

        [STAThread]
        static void Main(string[] args)
        {
            Application.Init();
            var top = Application.Top;
            var behaviorTypes = LoadBehaviorTypes();

            var win = new Window("World Object")
            {
                X = 0,
                Y = 1,
                Width = Dim.Fill(),
                Height = Dim.Fill()
            };

            top.Add(win);

            // Creates a menubar, the item "New" has a help menu.
            var menu = new MenuBar(new MenuBarItem[] {
                new MenuBarItem ("_File", new MenuItem [] {
                    new MenuItem ("_New", "Creates new file", null),
                    new MenuItem ("_Close", "", null),
                    new MenuItem ("_Quit", "", null)
                }),
                new MenuBarItem ("_Edit", new MenuItem [] {
                    new MenuItem ("_Copy", "", null),
                    new MenuItem ("C_ut", "", null),
                    new MenuItem ("_Paste", "", null)
                })
            });

            top.Add(menu);
        }

        private static IEnumerable<Type> LoadBehaviorTypes()
        {
            return typeof(DataDrivenBehavior<>).Assembly.GetTypes().Where(t => typeof(Behavior).IsAssignableFrom(t));
        }

        private static void AddWorldObjectControl() 
        {

        }
    }
}
