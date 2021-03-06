namespace Microsoft.Protocols.TestSuites.Common
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// RopGetPropertiesList request buffer structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RopGetPropertiesListRequest : ISerializable
    {
        /// <summary>
        /// Unsigned 8-bit integer. This value specifies the type of remote operation. 
        /// For this operation, this field is set to 0x09.
        /// </summary>
        public byte RopId;

        /// <summary>
        /// Unsigned 8-bit integer. This value specifies the logon associated with this operation.
        /// </summary>
        public byte LogonId;

        /// <summary>
        /// Unsigned 8-bit integer. This index specifies the location in the Server Object Handle Table 
        /// where the handle for the input Server Object is stored.
        /// </summary>
        public byte InputHandleIndex;

        /// <summary>
        /// Serialize the ROP request buffer.
        /// </summary>
        /// <returns>The ROP request buffer serialized.</returns>
        public byte[] Serialize()
        {
            byte[] serializeBuffer = new byte[Marshal.SizeOf(this)];
            IntPtr requestBuffer = new IntPtr();
            requestBuffer = Marshal.AllocHGlobal(Marshal.SizeOf(this));
            try
            {
                Marshal.StructureToPtr(this, requestBuffer, true);
                Marshal.Copy(requestBuffer, serializeBuffer, 0, Marshal.SizeOf(this));
                return serializeBuffer;
            }
            finally
            {
                Marshal.FreeHGlobal(requestBuffer);
            }
        }

        /// <summary>
        /// Return the size of this structure.
        /// </summary>
        /// <returns>The size of this structure.</returns>
        public int Size()
        {
            return Marshal.SizeOf(this);
        }
    }

    /// <summary>
    /// RopGetPropertiesList response buffer structure.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct RopGetPropertiesListResponse : IDeserializable
    {
        /// <summary>
        /// Unsigned 8-bit integer. This value specifies the type of remote operation. 
        /// For this operation, this field is set to 0x09.
        /// </summary>
        public byte RopId;

        /// <summary>
        /// Unsigned 8-bit integer. This index MUST be set to the InputHandleIndex specified in the request.
        /// </summary>
        public byte InputHandleIndex;

        /// <summary>
        /// Unsigned 32-bit integer. This value specifies the status of the remote operation. 
        /// For this response, this field is set to 0x00000000.
        /// </summary>
        public uint ReturnValue;

        /// <summary>
        /// Unsigned 16-bit integer. This value specifies the number of property tags in the PropertyTags field.
        /// </summary>
        public ushort PropertyTagCount;

        /// <summary>
        /// Array of PropertyTag structures. The number of structures contained in this field is specified 
        /// by the PropertyTagCount field. The format of the PropertyTag structure is specified in [MS-OXCDATA]. 
        /// This field lists the property tags on the object.
        /// </summary>
        public PropertyTag[] PropertyTags;

        /// <summary>
        /// Deserialize the ROP response buffer.
        /// </summary>
        /// <param name="ropBytes">ROPs bytes in response.</param>
        /// <param name="startIndex">The start index of this ROP.</param>
        /// <returns>The size of response buffer structure.</returns>
        public int Deserialize(byte[] ropBytes, int startIndex)
        {
            int index = startIndex;
            this.RopId = ropBytes[index++];
            this.InputHandleIndex = ropBytes[index++];
            this.ReturnValue = (uint)BitConverter.ToInt32(ropBytes, index);
            index += sizeof(uint);

            // Only success response has below fields
            if (this.ReturnValue == 0)
            {
                this.PropertyTagCount = (ushort)BitConverter.ToInt16(ropBytes, index);
                index += sizeof(ushort);
                if (this.PropertyTagCount > 0)
                {
                    this.PropertyTags = new PropertyTag[this.PropertyTagCount];
                    for (int i = 0; i < this.PropertyTagCount; i++)
                    {
                        index += this.PropertyTags[i].Deserialize(ropBytes, index);
                    }
                }
            }

            return index - startIndex;
        }
    }
}