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
using RAMvader;
using RAMvader.CodeInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

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
		/// <summary>The period of the timer used to look for the game's process, when the trainer is not attached. This number is given in miliseconds.</summary>
		private const int TIMER_LOOK_FOR_GAME_PROCESS_PERIOD_MS = 1000;
		#endregion





		#region PRIVATE FIELDS
		/// <summary>A Timer object used to periodically look for the game's process, when the trainer is not attached to the game.</summary>
		private Timer m_gameSearchTimer = null;
		/// <summary>A set containing all the cheats the user has enabled in the trainer.</summary>
		private HashSet<ECheat> m_enabledCheats = new HashSet<ECheat>();
		/// <summary>A memory alteration which is enabled when the trainer is attached to the game to keep track of the address of the player's
		/// current character's HP variable.</summary>
		private MemoryAlterationX86Call m_memAlterationStorePlayerHPAddress;
		#endregion





		#region PUBLIC PROPERTIES
		/// <summary>An object used for performing I/O operations on the game process' memory. </summary>
		public RAMvaderTarget GameMemoryIO { get; private set; }
		/// <summary>An object used for injecting and managing code caves and arbitrary variables into the
		/// game process' memory.</summary>
		public Injector<ECheat, ECodeCave, EVariable> GameMemoryInjector { get; private set; }
		#endregion





		#region PRIVATE METHODS
		/// <summary>Starts a timer which is responsible for periodically looking for the game's process.</summary>
		private void StartLookingForGameProcess()
		{
			// Define the callback function to be used for the timer 
			TimerCallback timerCallback = ( object state ) => {
				// Force timer's task to be executed in the SAME THREAD as the MainWindow
				this.Dispatcher.Invoke( () => {
					// This flag controls whether the timer used for looking for the game's process should be restarted or not
					bool bRestartLookForGameTimer = true;

					// Try to find the game's process
					Process gameProcess = Process.GetProcessesByName( GAME_PROCESS_NAME ).FirstOrDefault();
					if ( gameProcess != null )
					{
						// Try to attach to the game's process
						if ( GameMemoryIO.AttachToProcess( gameProcess ) )
						{
							// Inject the trainer's code and variables into the game's memory!
							GameMemoryInjector.Inject();

							// When the game's process exits, the MainWindow's dispatcher is used to invoke the DetachFromGame() method
							// in the same thread which "runs" our MainWindow
							GameMemoryIO.TargetProcess.EnableRaisingEvents = true;
							GameMemoryIO.TargetProcess.Exited += ( caller, args ) => {
								this.Dispatcher.Invoke( () => { this.DetachFromGame(); } );
							};

							// Build the cheat activation steps...
							IntPtr mainModuleAddress = GameMemoryIO.TargetProcess.MainModule.BaseAddress;

							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatHPHack, new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x5866B, ECodeCave.evCodeCaveHPHack, 6 ) );

							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatManaHack, new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x11B61E8, ECodeCave.evCodeCaveManaHack, 6 ) );

							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatSkillPointsHack, new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x14D60, ECodeCave.evCodeCaveSkillPointsHack1, 6 ) );
							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatSkillPointsHack, new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x14E90, ECodeCave.evCodeCaveSkillPointsHack2, 6 ) );
							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatSkillPointsHack, new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x14FC0, ECodeCave.evCodeCaveSkillPointsHack3, 6 ) );
							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatSkillPointsHack, new MemoryAlterationNOP( GameMemoryIO, mainModuleAddress + 0x14C46, 6 ) );
							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatSkillPointsHack, new MemoryAlterationNOP( GameMemoryIO, mainModuleAddress + 0x14D76, 6 ) );
							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatSkillPointsHack, new MemoryAlterationNOP( GameMemoryIO, mainModuleAddress + 0x14EA6, 6 ) );

							GameMemoryInjector.AddMemoryAlteration( ECheat.evCheatOneHitKillsHack, new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x5A22E4, ECodeCave.evCodeCaveOneHitKills, 6 ) );

							// Enable the code cave that keeps track of the address of the player's current character's HP variable
							m_memAlterationStorePlayerHPAddress = new MemoryAlterationX86Call( GameMemoryIO, mainModuleAddress + 0x117A80E, ECodeCave.evCodeCaveStorePtrCharHP, 6 );
							m_memAlterationStorePlayerHPAddress.SetEnabled( GameMemoryInjector, true );

							// Enable the cheats that the user has checked in the trainer's interface
							foreach ( ECheat curEnabledCheat in m_enabledCheats )
								GameMemoryInjector.SetMemoryAlterationsActive( curEnabledCheat, true );

							// The timer which looks for the game shouldn't be restarted, as the game has already been found
							bRestartLookForGameTimer = false;
						}
						else
						{
							MessageBox.Show( this,
								Properties.Resources.strMsgFailedToAttach, Properties.Resources.strMsgFailedToAttachCaption,
								MessageBoxButton.OK, MessageBoxImage.Exclamation );
						}
					}

					// RESTART the timer as a ONE-SHOT timer
					if ( bRestartLookForGameTimer )
						m_gameSearchTimer.Change( TIMER_LOOK_FOR_GAME_PROCESS_PERIOD_MS, Timeout.Infinite );
				} );
			};

			// Start the timer as a ONE-SHOT timer
			m_gameSearchTimer = new Timer( timerCallback, null, TIMER_LOOK_FOR_GAME_PROCESS_PERIOD_MS, Timeout.Infinite );
		}


		/// <summary>Called when the trainer needs to be detached from the game's process.</summary>
		private void DetachFromGame()
		{
			// Detach from the target process
			if ( GameMemoryIO.IsAttached() )
			{
				// If the game's process is still running, all cheats must be disabled
				if ( GameMemoryIO.TargetProcess.HasExited == false )
				{
					foreach ( ECheat curCheat in Enum.GetValues( typeof( ECheat ) ) )
						GameMemoryInjector.SetMemoryAlterationsActive( curCheat, false );
				}

				m_memAlterationStorePlayerHPAddress.SetEnabled( GameMemoryInjector, false );
				m_memAlterationStorePlayerHPAddress = null;

				// Release injected memory, cleanup and detach
				GameMemoryInjector.ResetAllocatedMemoryData();
				GameMemoryIO.DetachFromProcess();
			}

			// Restart the timer which looks for the game's process
			StartLookingForGameProcess();
		}
		#endregion




		#region PUBLIC METHODS
		/// <summary>Constructor.</summary>
		public MainWindow()
		{
			// Initialize objects which will perform operations on the game's memory
			GameMemoryIO = new RAMvaderTarget();

			GameMemoryInjector = new Injector<ECheat, ECodeCave, EVariable>();
			GameMemoryInjector.SetTargetProcess( GameMemoryIO );

			// Initialize RAMvaderTarget
			GameMemoryIO.SetTargetEndianness( EEndianness.evEndiannessLittle );
			GameMemoryIO.SetTargetPointerSize( EPointerSize.evPointerSize32 );

			// Initialize window
			InitializeComponent();

			// Start looking for the game!
			StartLookingForGameProcess();
		}
		#endregion





		#region EVENT HANDLERS
		/// <summary>Called when the #MainWindow of the trainer is about to be closed (but before actually closing it).</summary>
		/// <param name="sender">Object which sent the event.</param>
		/// <param name="e">Arguments from the event.</param>
		private void WindowClosing( object sender, CancelEventArgs e )
		{
			// Stop the timer which looks for the game's process, if it is running
			if ( m_gameSearchTimer != null )
				m_gameSearchTimer.Change( Timeout.Infinite, Timeout.Infinite );

			// Detach the trainer, if it is attached
			if ( GameMemoryIO.Attached )
				DetachFromGame();
		}


		/// <summary>
		/// Called when the user clicks the "Donate!" button.
		/// </summary>
		/// <param name="sender">Object which sent the event.</param>
		/// <param name="e">Arguments from the event.</param>
		private void ButtonClickDonate( object sender, RoutedEventArgs e )
		{
			Process.Start( PROJECT_DONATION_URL );
		}





		/// <summary>
		/// Called whenever any of the CheckBoxes used to activate/deactivate cheats from the trainer gets checked or unchecked.
		/// </summary>
		/// <param name="sender">Object which sent the event.</param>
		/// <param name="e">Arguments from the event.</param>
		private void CheckBoxCheatToggled( object sender, RoutedEventArgs e )
		{
			// Retrieve information which will be used to enable or disable the cheat
			CheckBox chkBox = (CheckBox) e.Source;
			ECheat cheatID = (ECheat) chkBox.Tag;
			bool bEnableCheat = ( chkBox.IsChecked == true );

			// Update the list of cheats to be enabled
			if ( bEnableCheat )
				m_enabledCheats.Add( cheatID );
			else
				m_enabledCheats.Remove( cheatID );

			// Enable or disable the cheat in the game's memory space
			if ( GameMemoryIO.IsAttached() )
				GameMemoryInjector.SetMemoryAlterationsActive( cheatID, bEnableCheat );
		}
		#endregion
	}
}
