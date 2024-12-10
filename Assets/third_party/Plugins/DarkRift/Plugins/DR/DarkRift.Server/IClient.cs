﻿/*
 * This Source Code Form is subject to the terms of the Mozilla Public
 * License, v. 2.0. If a copy of the MPL was not distributed with this
 * file, You can obtain one at https://mozilla.org/MPL/2.0/.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace DarkRift.Server
{
    /// <summary>
    ///     Server representation of a client.
    /// </summary>
    public interface IClient : IMessageSinkSource
    {
        /// <summary>
        ///     Called when the client is given a strike for illegal behaviour.
        /// </summary>
        event EventHandler<StrikeEventArgs> StrikeOccured;

        /// <summary>
        ///     The ID of the client.
        /// </summary>
        ushort ID { get; }

        /// <summary>
        ///     The state of the connection;
        /// </summary>
        ConnectionState ConnectionState { get; }

        /// <summary>
        ///     The time this client connected to the server.
        /// </summary>
        DateTime ConnectionTime { get; }

        /// <summary>
        ///     The number of messages sent from the server.
        /// </summary>
        uint MessagesSent { get; }

        /// <summary>
        ///     The number of messages pushed from the server.
        /// </summary>
        uint MessagesPushed { get; }

        /// <summary>
        ///     The number of messages received at the server.
        /// </summary>
        uint MessagesReceived { get; }

        /// <summary>
        ///     The collection of end points this client is connected to.
        /// </summary>
        IEnumerable<IPEndPoint> RemoteEndPoints { get; }

        /// <summary>
        ///     Disconnects this client from the server.
        /// </summary>
        /// <returns>Whether the disconnect was successful.</returns>
        bool Disconnect(string reason);

        /// <summary>
        ///     Gets the remote end point with the given name.
        /// </summary>
        /// <param name="name">The end point name.</param>
        /// <returns>The end point.</returns>
        IPEndPoint GetRemoteEndPoint(string name);

        #region Strikes

        /// <summary>
        ///     Strikes this client.
        /// </summary>
        /// <param name="message">A message describing the reason for the strike.</param>
        void Strike(string message = null);

        #endregion
    }
}
