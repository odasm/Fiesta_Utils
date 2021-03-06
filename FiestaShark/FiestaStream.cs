﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MapleShark {
    public sealed class FiestaStream {
        // Fields
        private const int DEFAULT_SIZE = 0x1000;
        private byte[] mBuffer = new byte[0x1000];
        private int mCursor = 0;
        public bool mOutbound;
        public int xorpos = 0;
        public DateTime Created;

        private static byte[] xorTable = new byte[] { 
            7, 0x59, 0x69, 0x4a, 0x94, 0x11, 0x94, 0x85, 140, 0x88, 5, 0xcb, 160, 0x9e, 0xcd, 0x58, 
            0x3a, 0x36, 0x5b, 0x1a, 0x6a, 0x16, 0xfe, 0xbd, 0xdf, 0x94, 2, 0xf8, 0x21, 150, 200, 0xe9, 
            0x9e, 0xf7, 0xbf, 0xbd, 0xcf, 0xcd, 0xb2, 0x7a, 0, 0x9f, 0x40, 0x22, 0xfc, 0x11, 0xf9, 12, 
            0x2e, 0x12, 0xfb, 0xa7, 0x74, 10, 0x7d, 120, 0x40, 30, 0x2c, 160, 0x2d, 6, 0xcb, 0xa8, 
            0xb9, 0x7e, 0xef, 0xde, 0x49, 0xea, 0x4e, 0x13, 0x16, 0x16, 0x80, 0xf4, 0x3d, 0xc2, 0x9a, 0xd4, 
            0x86, 0xd7, 0x94, 0x24, 0x17, 0xf4, 0xd6, 0x65, 0xbd, 0x3f, 0xdb, 0xe4, 0xe1, 15, 80, 0xf6, 
            0xec, 0x7a, 0x9a, 12, 0x27, 0x3d, 0x24, 0x66, 0xd3, 0x22, 0x68, 0x9c, 0x9a, 0x52, 11, 0xe0, 
            0xf9, 0xa5, 11, 0x25, 0xda, 0x80, 0x49, 13, 0xfd, 0x3e, 0x77, 0xd1, 0x56, 0xa8, 0xb7, 0xf4, 
            15, 0x9b, 0xe8, 15, 0x52, 0x47, 0xf5, 0x6f, 0x83, 0x20, 0x22, 0xdb, 15, 11, 0xb1, 0x43, 
            0x85, 0xc1, 0xcb, 0xa4, 11, 2, 0x19, 0xdf, 240, 0x8b, 0xec, 0xdb, 0x6c, 0x6d, 0x66, 0xad, 
            0x45, 190, 0x89, 20, 0x7e, 0x2f, 0x89, 0x10, 0xb8, 0x93, 0x60, 0xd8, 0x60, 0xde, 0xf6, 0xfe, 
            110, 0x9b, 0xca, 6, 0xc1, 0x75, 0x95, 0x33, 0xcf, 0xc0, 0xb2, 0xe0, 0xcc, 0xa5, 0xce, 0x12, 
            0xf6, 0xe5, 0xb5, 180, 0x26, 0xc5, 0xb2, 0x18, 0x4f, 0x2a, 0x5d, 0x26, 0x1b, 0x65, 0x4d, 0xf5, 
            0x45, 0xc9, 0x84, 20, 220, 0x7c, 0x12, 0x4b, 0x18, 0x9c, 0xc7, 0x24, 0xe7, 60, 100, 0xff, 
            0xd6, 0x3a, 0x2c, 0xee, 140, 0x81, 0x49, 0x39, 0x6c, 0xb7, 220, 0xbd, 0x94, 0xe2, 50, 0xf7, 
            0xdd, 10, 0xfc, 2, 1, 100, 0xec, 0x4c, 0x94, 10, 0xb1, 0x56, 0xf5, 0xc9, 0xa9, 0x34, 
            0xde, 15, 0x38, 0x27, 0xbc, 0x81, 0x30, 15, 0x7b, 0x38, 0x25, 0xfe, 0xe8, 0x3e, 0x29, 0xba, 
            0x55, 0x43, 0xbf, 0x6b, 0x9f, 0x1f, 0x8a, 0x49, 0x52, 0x18, 0x7f, 0x8a, 0xf8, 0x88, 0x24, 0x5c, 
            0x4f, 0xe1, 0xa8, 0x30, 0x87, 0x8e, 80, 0x1f, 0x2f, 0xd1, 12, 180, 0xfd, 10, 0xbc, 220, 
            0x12, 0x85, 0xe2, 0x52, 0xee, 0x4a, 0x58, 0x38, 0xab, 0xff, 0xc6, 0x3d, 0xb9, 0x60, 100, 10, 
            180, 80, 0xd5, 0x40, 0x89, 0x17, 0x9a, 0xd5, 0x85, 0xcf, 0xec, 13, 0x7e, 0x81, 0x7f, 0xe3, 
            0xc3, 4, 1, 0x22, 0xec, 0x27, 0xcc, 250, 0x3e, 0x21, 0xa6, 0x54, 200, 0xde, 0, 0xb6, 
            0xdf, 0x27, 0x9f, 0xf6, 0x25, 0x34, 7, 0x85, 0xbf, 0xa7, 0xa5, 0xa5, 0xe0, 0x83, 12, 0x3d, 
            0x5d, 0x20, 0x40, 0xaf, 0x60, 0xa3, 100, 0x56, 0xf3, 5, 0xc4, 0x1c, 0x7d, 0x37, 0x98, 0xc3, 
            0xe8, 90, 110, 0x58, 0x85, 0xa4, 0x9a, 0x6b, 0x6a, 0xf4, 0xa3, 0x7b, 0x61, 0x9b, 9, 0x40, 
            30, 0x60, 0x4b, 50, 0xd9, 0x51, 0xa4, 0xfe, 0xf9, 0x5d, 0x4e, 0x4a, 0xfb, 0x4a, 0xd4, 0x7c, 
            0x33, 2, 0x33, 0xd5, 0x9d, 0xce, 0x5b, 170, 90, 0x7c, 0xd8, 0xf8, 5, 250, 0x1f, 0x2b, 
            140, 0x72, 0x57, 80, 0xae, 0x6c, 0x19, 0x89, 0xca, 1, 0xfc, 0xfc, 0x29, 0x9b, 0x61, 0x12, 
            0x68, 0x63, 0x65, 70, 0x26, 0xc4, 0x5b, 80, 170, 0x2b, 190, 0xef, 0x9a, 0x79, 2, 0x23, 
            0x75, 0x2c, 0x20, 0x13, 0xfd, 0xd9, 90, 0x76, 0x23, 0xf1, 11, 0xb5, 0xb8, 0x59, 0xf9, 0x9f, 
            0x7a, 230, 6, 0xe9, 0xa5, 0x3a, 180, 80, 0xbf, 0x16, 0x58, 0x98, 0xb3, 0x9a, 110, 0x36, 
            0xee, 0x8d, 0xeb
         };

        // Methods
        public FiestaStream(short xorpos, bool Outbound) {
            this.xorpos = xorpos;
            this.mOutbound = Outbound;
            Created = DateTime.Now;
        }

        public void Append(byte[] pBuffer) {
            this.Append(pBuffer, 0, pBuffer.Length);
        }

        public void Append(byte[] pBuffer, int pStart, int pLength) {
            if( (this.mBuffer.Length - this.mCursor) < pLength ) {
                int newSize = this.mBuffer.Length * 2;
                while( newSize < (this.mCursor + pLength) ) {
                    newSize *= 2;
                }
                Array.Resize<byte>(ref this.mBuffer, newSize);
            }
            Buffer.BlockCopy(pBuffer, pStart, this.mBuffer, this.mCursor, pLength);
            this.mCursor += pLength;
        }

        Mutex decryptor = new Mutex();
        private void Decrypt(byte[] pBuffer) {
            try {
                decryptor.WaitOne();
                for( int i = 0; i < pBuffer.Length; ++i ) {
                    pBuffer[i] = (byte)(pBuffer[i] ^ xorTable[this.xorpos]);
                    this.xorpos++;
                    if( this.xorpos == 499 ) {
                        this.xorpos = 0;
                    }
                }
            } finally {
                decryptor.ReleaseMutex();
            }
        }

        public FiestaPacket Read(DateTime pTransmitted) {
            if( this.mCursor < 1 ) {
                return null;
            }
            short count = this.mBuffer[0];
            bool BigPacket = false;
            if( count == 0 ) {
                if( mCursor < 2 ) return null;
                count = BitConverter.ToInt16(this.mBuffer, 1);
                BigPacket = true;
            }
            if( BigPacket && mCursor < count + 3 ) return null; //00 short
            if( !BigPacket && mCursor < count + 1 ) return null; //byte
            byte[] dst = new byte[count];
            Buffer.BlockCopy(this.mBuffer, !BigPacket ? 1 : 3, dst, 0, count);
            if( this.mOutbound ) this.Decrypt(dst);
            this.mCursor -= !BigPacket ? (count + 1) : (count + 3);
            if( this.mCursor > 0 ) {
                Buffer.BlockCopy(this.mBuffer, !BigPacket ? (count + 1) : (count + 3), this.mBuffer, 0, this.mCursor);
            }
            ushort opcode = (ushort)(dst[0] | (dst[1] << 8));
            Buffer.BlockCopy(dst, 2, dst, 0, count - 2);
            Array.Resize<byte>(ref dst, count - 2);
            Definition definition = Config.Instance.Definitions.Find(delegate(Definition d) {
                return (d.Outbound == this.mOutbound) && (d.Opcode == opcode);
            });

            return new FiestaPacket(pTransmitted, this.mOutbound, opcode, (definition == null) ? "" : definition.Name, dst, Created);
        }
    }
}
