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

/* This file keeps definitions for code elements which are part of the low-level features of the trainer. */
using RAMvader.CodeInjection;
using System;

namespace RAMvader_Trine_EnchantedEdition
{
	/// <summary>Identifiers for all cheats available in the trainer.</summary>
	public enum ECheat
	{
		/// <summary>Identifies the "HP Hack" cheat.</summary>
		evCheatHPHack,
		/// <summary>Identifies the "Mana Hack" cheat.</summary>
		evCheatManaHack,
		/// <summary>Identifies the "One Hit Kills" cheat.</summary>
		evCheatOneHitKillsHack,
		/// <summary>Identifies the "Skill Points Hack" cheat.</summary>
		evCheatSkillPointsHack,
	}





	/// <summary>Identifiers for all of the code caves injected into the game process' memory space,
	/// once the trainer gets attached to the game.</summary>
	public enum ECodeCave
	{
		/// <summary>Identifies the code cave used to keep track of the pointer of the player's current character's HP variable.</summary>
		[CodeCaveDefinition( 0x52, 0x8D, 0x90, 0x60, 0x01, 0x00, 0x00, 0x89, 0x15, EVariable.evVarPtrCurrentCharHP, 0x5A, 0x8B, 0x88, 0x60, 0x01, 0x00, 0x00, 0xC3 )]
		evCodeCaveStorePtrCharHP,
		/// <summary>Identifies the code cave used for the "HP Hack" cheat.</summary>
		[CodeCaveDefinition( 0x50, 0x31, 0xC0, 0xB0, 0xC8, 0x89, 0x86, 0x60, 0x01, 0x00, 0x00, 0x8B, 0x96, 0x60, 0x01, 0x00, 0x00, 0x58, 0xC3 )]
		evCodeCaveHPHack,
		/// <summary>Identifies the code cave used for the "Mana Hack" cheat.</summary>
		[CodeCaveDefinition( 0x31, 0xF6, 0x66, 0xBE, 0x9A, 0x02, 0x89, 0xB3, 0xF0, 0x00, 0x00, 0x00, 0xC3 )]
		evCodeCaveManaHack,
		/// <summary>Identifies the first code cave used for the "Skill Points Hack" cheat.</summary>
		[CodeCaveDefinition( 0x52, 0x31, 0xD2, 0xB2, 0x7B, 0x89, 0x91, 0x3C, 0x02, 0x00, 0x00, 0x8B, 0x81, 0x3C, 0x02, 0x00, 0x00, 0x5A, 0xC3 )]
		evCodeCaveSkillPointsHack1,
		/// <summary>Identifies the second code cave used for the "Skill Points Hack" cheat.</summary>
		[CodeCaveDefinition( 0x52, 0x31, 0xD2, 0xB2, 0x7B, 0x89, 0x91, 0x40, 0x02, 0x00, 0x00, 0x8B, 0x81, 0x40, 0x02, 0x00, 0x00, 0x5A, 0xC3 )]
		evCodeCaveSkillPointsHack2,
		/// <summary>Identifies the third code cave used for the "Skill Points Hack" cheat.</summary>
		[CodeCaveDefinition( 0x52, 0x31, 0xD2, 0xB2, 0x7B, 0x89, 0x91, 0x44, 0x02, 0x00, 0x00, 0x8B, 0x81, 0x44, 0x02, 0x00, 0x00, 0x5A, 0xC3 )]
		evCodeCaveSkillPointsHack3,
		[CodeCaveDefinition( 0x53, 0x52, 0x8D, 0x97, 0x60, 0x01, 0x00, 0x00, 0x8B, 0x1D, EVariable.evVarPtrCurrentCharHP, 0x39, 0xDA, 0x74, 0x04,
			0x31, 0xC0, 0xEB, 0x06, 0x8B, 0x87, 0x60, 0x01, 0x00, 0x00, 0x5A, 0x5B, 0xC3 )]
		evCodeCaveOneHitKills,
	}





	/// <summary>Identifiers for all variables injected into the game process' memory space,
	/// once the trainer gets attached to the game.</summary>
	public enum EVariable
	{
		/// <summary>Identifies the variable which stores the pointer to the player's current character's HP.</summary>
		[VariableDefinition( (UInt32) 0 )]
		evVarPtrCurrentCharHP,
	}
}