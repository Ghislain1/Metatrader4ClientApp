using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

using MahApps.Metro.Controls;

using Metatrader4ClientApp.Infrastructure.Interfaces;

using Prism.Regions;

namespace Metatrader4ClientApp.Adapters
{
    
        /// <summary>
        /// Defines the <see cref="HamburgerMenuItemCollectionRegionAdapter"/>.
        /// </summary>
        public class HamburgerMenuItemCollectionRegionAdapter : RegionAdapterBase<HamburgerMenuItemCollection>
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="HamburgerMenuItemCollectionRegionAdapter"/> class.
            /// </summary>
            /// <param name="regionBehaviorFactory">The regionBehaviorFactory <see cref="IRegionBehaviorFactory"/>.</param>
            public HamburgerMenuItemCollectionRegionAdapter(IRegionBehaviorFactory regionBehaviorFactory)
                : base(regionBehaviorFactory)
            {
            }

            /// <summary>
            /// The Adapt.
            /// </summary>
            /// <param name="region">The region <see cref="IRegion"/>.</param>
            /// <param name="regionTarget">The regionTarget <see cref="HamburgerMenuItemCollection"/>.</param>
            protected override void Adapt(IRegion region, HamburgerMenuItemCollection regionTarget)
            {
                region.Views.CollectionChanged += (s, e) =>
                {
                    switch (e.Action)
                    {
                        case System.Collections.Specialized.NotifyCollectionChangedAction.Add:
                            foreach (var element in e.NewItems)
                            {
                                var plugin = ((UserControl)element).DataContext as IPlugin;
                                var hamburgerMenuGlyphItem = new HamburgerMenuGlyphItem();
                               // hamburgerMenuGlyphItem.Glyph = plugin.Glyph;
                                
                                hamburgerMenuGlyphItem.Label = plugin.Label;
                                hamburgerMenuGlyphItem.CommandParameter = plugin.CommandParameter;
                                hamburgerMenuGlyphItem.Tag = (UserControl)element;
                                hamburgerMenuGlyphItem.CommandParameter = element.GetType();
                                regionTarget.Add(hamburgerMenuGlyphItem);
                                hamburgerMenuGlyphItem.IsVisible = true;
                            }
                            break;

                        case System.Collections.Specialized.NotifyCollectionChangedAction.Remove:
                            foreach (var t in e.OldItems)
                            {
                                for (var i = 0; i < regionTarget.Count; i++)
                                {
                                    var tab = regionTarget[i];
                                    if (tab.Tag == e.OldItems[0])
                                    {
                                        regionTarget.Remove(tab);
                                    }
                                }

                            }
                            break;
                    }
                };
            }

            /// <summary>
            /// The CreateRegion.
            /// </summary>
            /// <returns>The <see cref="IRegion"/>.</returns>
            protected override IRegion CreateRegion()
            {
                return new AllActiveRegion();
            }
        }
    }
