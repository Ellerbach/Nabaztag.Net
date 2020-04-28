// Licensed to Laurent Ellerbach under one or more agreements.
// Laurent Ellerbach licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Nabaztag.Net.Models;
using Newtonsoft.Json;
using System;
using Xunit;

namespace Nabaztag.Net.Test
{
    public class DeserializeObjectFromServer
    {
        [Fact]
        public void DeserializeStateObject()
        {
            var input = "{\"type\":\"state\",\"state\":\"idle\"}";
            var state = JsonConvert.DeserializeObject<NabState>(input);
            Assert.Equal(StateType.Idle, state.State);
        }

        [Fact]
        public void DeserializeEarEventEarsLeftAndRight()
        {
            const int left = -1;
            const int right = 2;
            var input = $"{{\"type\":\"ears_event\",\"left\":{left},\"right\":{right}}}";
            var ears = JsonConvert.DeserializeObject<EarsEvent>(input);
            Assert.Equal(ears.Left, left);
            Assert.Equal(ears.Right, right);
            Assert.Null(ears.Ear);
        }

        [Fact]
        public void DeserializeEarEventEar()
        {
            var ear = Ear.Left;
            var input = $"{{\"type\":\"ears_event\",\"ear\":\"{ear.ToString().ToLower()}\"}}";
            var ears = JsonConvert.DeserializeObject<EarsEvent>(input);
            Assert.Null(ears.Left);
            Assert.Null(ears.Right);
            Assert.Equal(ear, ears.Ear);
        }
    }
}
