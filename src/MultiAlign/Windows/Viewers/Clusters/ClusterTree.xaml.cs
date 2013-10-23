using System.Collections.Generic;
using System.Windows.Controls;
using MultiAlign.ViewModels;
using MultiAlignCore.Data.Features;

namespace MultiAlign.Windows.Viewers.Clusters
{
    /// <summary>
    /// Interaction logic for ClusterTree.xaml
    /// </summary>
    public partial class ClusterTree : UserControl
    {
        UMCClusterCollectionViewModel m_clusters;

        public ClusterTree()
        {
            InitializeComponent();            
        }

        public void SetClusters(List<UMCClusterLightMatched> clusters)
        {
            m_clusters  = new UMCClusterCollectionViewModel(clusters);
            DataContext = m_clusters;
        }
    }
}


 //<!-- Peptides -->
 //               <HierarchicalDataTemplate 
 //                         DataType="{x:Type viewModels:DatabaseSearchIdentificationsTreeViewModel}" 
 //                         ItemsSource="{Binding Peptides}">
 //                       <Label>Identifications</Label>
 //               </HierarchicalDataTemplate>

                

 //               <!-- Peptides -->
 //               <HierarchicalDataTemplate 
 //                         DataType="{x:Type viewModels:MassTagMatchViewModel}" 
 //                         ItemsSource="{Binding MassTags}">
 //                       <Label>Identifications</Label>
 //               </HierarchicalDataTemplate>

 //               <!-- Spectra -->
 //               <HierarchicalDataTemplate 
 //                       DataType="{x:Type viewModels:MsMsCollectionTreeViewModel}" 
 //                       ItemsSource="{Binding Peptides}">
 //                   <Label>MS / MS</Label>
 //               </HierarchicalDataTemplate>



 //                   <!-- UMC View Model --> 
 //               <HierarchicalDataTemplate 
 //                 DataType="{x:Type viewModels:MassTagCollectionMatchViewModel}" 
 //                 ItemsSource="{Binding Features}">
 //                   <Border CornerRadius="2" BorderThickness="1" HorizontalAlignment="Stretch" Padding="3" Margin="3" BorderBrush="#FFCCCCCC">
 //                       <StackPanel Orientation="Horizontal">
 //                           <Image
 //                               Width="16"
 //                               Height="16"
 //                               Source="/MultiAlign;component/Resources/molecule.png"
 //                               />
 //                           <TextBlock 
 //                               HorizontalAlignment="Stretch">
 //                               <TextBlock.Text>                                                            
 //                                       <Binding Path="Id" />                    
 //                               </TextBlock.Text>
 //                           </TextBlock>
                            
 //                           <Label
 //                               VerticalContentAlignment="Center">
 //                               Mass:
 //                           </Label>
 //                           <Label
 //                               VerticalContentAlignment="Center"
 //                               Content="{Binding Path=Mass}">
 //                           </Label>
                            
 //                           <Label
 //                               VerticalContentAlignment="Center">
 //                               NET:
 //                           </Label>
 //                           <Label
 //                               VerticalContentAlignment="Center"
 //                               Content="{Binding Path=Net}">
 //                           </Label>

 //                           <Label
 //                               VerticalContentAlignment="Center">
 //                               MS/MS Count:
 //                           </Label>
 //                           <Label
 //                               VerticalContentAlignment="Center"
 //                               Content="{Binding Path=MsMsCount}">
 //                           </Label>
 //                           </StackPanel>
 //                   </Border>
                
 //               </HierarchicalDataTemplate>