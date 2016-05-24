/*
 * Copyright (C) 2014 Vinicius Rogério Araujo Silva
 *
 * This file is part of RAMvader-Trine-EnchantedEdition.
 * 
 * RAMvader-Trine-EnchantedEdition is free software: you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation, either version 3 of the License, or
 * (at your option) any later version.
 * 
 * RAMvader-Trine-EnchantedEdition is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with RAMvader-Trine-EnchantedEdition.  If not, see <http://www.gnu.org/licenses/>.
 */
using System.Diagnostics;
using System.Windows;

namespace RAMvader_Trine_EnchantedEdition
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		#region PRIVATE CONSTANTS
		/// <summary>The name of the process which runs the game.</summary>
		private const string GAME_PROCESS_NAME = "trine1_32bit";
		/// <summary>The URL which points to the webpage that runs when the user clicks the "Donate!" button of the trainer.</summary>
		private const string PROJECT_DONATION_URL = "https://www.paypal.com/cgi-bin/webscr?cmd=_donations&business=WJ2D2KRMPRKBS&lc=US&item_name=Supporting%20Vinicius%2eRAS%27%20open%20source%20projects&currency_code=USD&bn=PP%2dDonationsBF%3abtn_donate_LG%2egif%3aNonHosted";
		#endregion





		#region PUBLIC METHODS
		/// <summary>Constructor.</summary>
		public MainWindow()
		{
			InitializeComponent();
		}
		#endregion

		/// <summary>
		/// Called when the user clicks the "Donate!" button.
		/// </summary>
		/// <param name="sender">Object which sent the event.</param>
		/// <param name="e">Arguments from the event.</param>
		private void ButtonClickDonate( object sender, RoutedEventArgs e )
		{
			Process.Start( PROJECT_DONATION_URL );
		}
	}
}
