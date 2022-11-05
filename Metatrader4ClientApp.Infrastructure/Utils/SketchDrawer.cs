// <copyright company="ROSEN Swiss AG">
//  Copyright (c) ROSEN Swiss AG
//  This computer program includes confidential, proprietary
//  information and is a trade secret of ROSEN. All use,
//  disclosure, or reproduction is prohibited unless authorized in
//  writing by an officer of ROSEN. All Rights Reserved.
// </copyright>

namespace Metatrader4ClientApp.Infrastructure.Utils
{
    using Prism.Mvvm;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;

    /// <summary>
    ///    This class defines the attached property and related change the usercontrol on draft state
    /// </summary>
    public static class SketchDrawer
    {
        //
        // Summary:
        //     The AutoWireViewModel attached property.
        public static DependencyProperty IsOnDraftStateProperty = DependencyProperty.RegisterAttached("IsOnDraftState", typeof(bool?), typeof(SketchDrawer), new PropertyMetadata(null, IsOnDraftStateChanged));

        public static void SetIsOnDraftState(DependencyObject obj, bool? value)
        {
            obj.SetValue(IsOnDraftStateProperty, value);
        }

        public static bool? GetIsOnDraftState(DependencyObject obj)
        {
            return (bool?)obj.GetValue(IsOnDraftStateProperty);
        }
        private static void IsOnDraftStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(d))
            {
                bool? flag = (bool?)e.NewValue;
                if (flag.HasValue && flag.Value)
                {
                    if (d is UserControl userControl)
                    {
                        userControl.Loaded += OnLoaded;
                        

                    }
                }
            }

            void OnLoaded(object sender, RoutedEventArgs e)
            {
                if (d is not UserControl userControl)
                {
                    return;
                }
                if(userControl.Content is not  UIElement content)
                {
                    return;
                }
              
                // userControl.
                Grid grid = new Grid();                
               // grid.Children.Add(content);
                grid.Children.Add(GetDraftSymbol());
                userControl.Content = grid;
                userControl.Loaded -= OnLoaded;
            }
        }



        private static UIElement GetDraftSymbol()
        {
            return new TextBlock()
            {
                Text = "Draft...",
                FontFamily = new FontFamily("Verdana"),
                FontSize = 40,                
                HorizontalAlignment=HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
                TextAlignment = TextAlignment.Center,
                RenderTransformOrigin = new Point(0.5, 0.5),
                LayoutTransform = new RotateTransform(-45)
            };


        }
    }
}
