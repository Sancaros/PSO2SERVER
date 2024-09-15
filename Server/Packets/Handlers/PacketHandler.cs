﻿using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;

namespace PSO2SERVER.Packets.Handlers
{
    public class PacketHandlerAttr : Attribute
    {
        public uint Type, Subtype;

        public PacketHandlerAttr(uint type, uint subtype)
        {
            Type = type;
            Subtype = subtype;
        }
    }

    public abstract class PacketHandler
    {
        public abstract void HandlePacket(Client context, byte flags, byte[] data, uint position, uint size);
    }

    public static class PacketHandlers
    {
        private static readonly Dictionary<ushort, PacketHandler> Handlers = new Dictionary<ushort, PacketHandler>();

        public static void LoadPacketHandlers()
        {
            var handlers = (from t in Assembly.GetExecutingAssembly().GetTypes()
                            where t.IsClass && t.Namespace == "PSO2SERVER.Packets.Handlers" &&
                                  t.IsSubclassOf(typeof(PacketHandler))
                            let attrs = (PacketHandlerAttr[])t.GetCustomAttributes(typeof(PacketHandlerAttr), false)
                            where attrs.Length > 0
                            select new
                            {
                                Type = attrs[0].Type,
                                Subtype = attrs[0].Subtype,
                                HandlerType = t
                            }).ToList();

            // Sort handlers by Type and Subtype
            handlers = handlers.OrderBy(h => h.Type).ThenBy(h => h.Subtype).ToList();

            foreach (var handler in handlers)
            {
                Logger.WriteInternal("[数据] 数据包 0x{0:X2} - 0x{1:X2} 处理已载入 {2} ."
                    , handler.Type
                    , handler.Subtype
                    , handler.HandlerType.Name
                    );

                ushort packetTypeUShort = Helper.PacketTypeToUShort(handler.Type, handler.Subtype);
                if (!Handlers.ContainsKey(packetTypeUShort))
                {
                    Handlers.Add(packetTypeUShort, (PacketHandler)Activator.CreateInstance(handler.HandlerType));
                }
            }
        }

        /// <summary>
        ///     Gets and creates a PacketHandler for a given packet type and subtype.
        /// </summary>
        /// <returns>An instance of a PacketHandler or null</returns>
        /// <param name="type">Type a.</param>
        /// <param name="subtype">Type b.</param>
        public static PacketHandler GetHandlerFor(uint type, uint subtype)
        {
            var packetCode = Helper.PacketTypeToUShort(type, subtype);
            PacketHandler handler = null;

            if (Handlers.ContainsKey(packetCode))
                Handlers.TryGetValue(packetCode, out handler);

            return handler;
        }

        public static PacketHandler[] GetLoadedHandlers()
        {
            return Handlers.Values.ToArray();
        }
    }
}