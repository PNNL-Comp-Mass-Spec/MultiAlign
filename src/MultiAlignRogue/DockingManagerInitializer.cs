namespace MultiAlignRogue
{
    using Xceed.Wpf.AvalonDock;
    using Xceed.Wpf.AvalonDock.Layout;
    using Xceed.Wpf.AvalonDock.Layout.Serialization;

    /// <summary>
    /// This class is an extension of docking manager.
    /// I did this to decouple the deserialization and initialization of the docking manager.
    /// AvalonDock performs these actions together as part of its XmlLayoutSerialization class.
    /// I wanted to serialize this separately so I could serialize the view along with other view-related settings.
    /// </summary>
    public static class DockingManagerInitializer
    {
        public static void Initialize(this DockingManager manager, LayoutRoot layout)
        {
            var layoutSerializer = new ConcreteLayoutSerializer(manager);
            try
            {
                layoutSerializer.Start();
                layoutSerializer.Fixup(layout);
                manager.Layout = layout;
            }
            finally
            {
                layoutSerializer.End();
            }
        }

        /// <summary>
        /// This class exists to implement LayoutSerializer because it is abstract.
        /// </summary>
        private class ConcreteLayoutSerializer : LayoutSerializer
        {
            public ConcreteLayoutSerializer(DockingManager manager)
                : base(manager)
            {
            }

            public void Start()
            {
                this.StartDeserialization();
            }

            public void Fixup(LayoutRoot layout)
            {
                this.FixupLayout(layout);
            }

            public void End()
            {
                this.EndDeserialization();
            }
        }
    }
}
