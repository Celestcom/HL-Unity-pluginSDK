/* This code is licensed under the NullSpace Developer Agreement, available here:
** ***********************
** http://nullspacevr.com/?wpdmpro=nullspace-developer-agreement
** ***********************
** Make sure that you have read, understood, and agreed to the Agreement before using the SDK
*/
using System;
namespace NullSpace.API
{
    /// <summary>
    /// A class that satisfies the PacketConsumer interface will be able to consume packets generated by the 
    /// PacketDispatcher
    /// </summary>
	public interface PacketConsumer
	{
        /// <summary>
        /// Consume a raw packet
        /// </summary>
        /// <param name="packet"></param>
		void ConsumePacket (byte[] packet);
	}
}
