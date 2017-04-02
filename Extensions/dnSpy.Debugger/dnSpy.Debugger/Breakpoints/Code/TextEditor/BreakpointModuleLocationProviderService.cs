﻿/*
    Copyright (C) 2014-2017 de4dot@gmail.com

    This file is part of dnSpy

    dnSpy is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    dnSpy is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with dnSpy.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using dnSpy.Contracts.Debugger.Breakpoints.Code;
using dnSpy.Contracts.Debugger.Breakpoints.Code.TextEditor;
using dnSpy.Contracts.Text.Editor;

namespace dnSpy.Debugger.Breakpoints.Code.TextEditor {
	abstract class BreakpointModuleLocationProviderService {
		public abstract GlyphTextMarkerLocationInfo GetLocation(DbgCodeBreakpoint breakpoint);
	}

	[Export(typeof(BreakpointModuleLocationProviderService))]
	sealed class BreakpointModuleLocationProviderServiceImpl : BreakpointModuleLocationProviderService {
		readonly Lazy<BreakpointModuleLocationProvider, IBreakpointModuleLocationProviderMetadata>[] breakpointModuleLocationProviders;

		[ImportingConstructor]
		BreakpointModuleLocationProviderServiceImpl([ImportMany] IEnumerable<Lazy<BreakpointModuleLocationProvider, IBreakpointModuleLocationProviderMetadata>> breakpointModuleLocationProviders) =>
			this.breakpointModuleLocationProviders = breakpointModuleLocationProviders.OrderBy(a => a.Metadata.Order).ToArray();

		public override GlyphTextMarkerLocationInfo GetLocation(DbgCodeBreakpoint breakpoint) {
			if (breakpoint == null)
				throw new ArgumentNullException(nameof(breakpoint));
			foreach (var lz in breakpointModuleLocationProviders) {
				var loc = lz.Value.GetLocation(breakpoint);
				if (loc != null)
					return loc;
			}
			return null;
		}
	}
}
