#region License

// A simplistic Behavior Tree implementation in C#
// Copyright (C) 2010-2011 ApocDev apocdev@gmail.com
// 
// This file is part of TreeSharp
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.

// TODO: THIS WAS A NEW FILE -- MODIFY THIS HEADER
#endregion

using System;
using System.Collections.Generic;
using UnityEngine;

namespace TreeSharpPlus
{
	/// <summary>
	/// One node at random is executed.  The returned RunStatus
	/// is equal to that of the randomly chosen node.
	/// </summary>
	public class RandomNode : Parallel
	{
		private int runningNodes;
		private int chosenNode;

		private static System.Random rnd = new System.Random ();
		
		public RandomNode(params Node[] children)
			: base(children)
		{
		}
		
		public override void Start()
		{
			int totalNodes = this.Children.Count;
			chosenNode = rnd.Next (0, totalNodes);
			
			this.Children [chosenNode].Start ();
			
			base.Start();
		}
		
		public override void Stop()
		{
			// Stop all children
			this.Children [chosenNode].Stop ();
			base.Stop();
		}
		
		public override IEnumerable<RunStatus> Execute()
		{
			while (true)
			{
				RunStatus tickResult = RunStatus.Success;
				
				Node node = this.Children[chosenNode];
				tickResult = this.TickNode(node);
				
				// Check to see if anything finished
				if (tickResult == RunStatus.Running) {
					yield return RunStatus.Running;
				} else {
					// Clean up the node
					node.Stop();
					this.childStatus[chosenNode] = tickResult;
					
					yield return tickResult;
					yield break;
				}
				
				
			}
		}
	}
}