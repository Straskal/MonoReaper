using System.Text;
using System;
using Microsoft.Xna.Framework;

namespace Adventure
{
    public class Message
    {
        private readonly byte[] _buffer;
        private int _writePosition;
        private int _readPosition;

        public Message()
        {
            _buffer = new byte[1024];
            _writePosition = 0;
            _readPosition = 0;
        }

        public Message(byte[] buffer)
        {
            _buffer = buffer;
            _writePosition = 0;
            _readPosition = 0;
        }

        public ReadOnlySpan<byte> ToReadonlySpan()
        {
            return _buffer.AsSpan(0, _writePosition);
        }

        public void Write(byte value)
        {
            _buffer[_writePosition++] = value;
        }

        public byte ReadByte()
        {
            return _buffer[_readPosition++];
        }

        public void Write(int value)
        {
            var bytes = BitConverter.GetBytes(value);
            _buffer[_writePosition + 0] = bytes[0];
            _buffer[_writePosition + 1] = bytes[1];
            _buffer[_writePosition + 2] = bytes[2];
            _buffer[_writePosition + 3] = bytes[3];
            _writePosition += 4;
        }

        public int ReadInt()
        {
            var result = BitConverter.ToInt32(_buffer, _readPosition);
            _readPosition += 4;
            return result;
        }

        public void Write(float value)
        {
            var bytes = BitConverter.GetBytes(value);
            _buffer[_writePosition + 0] = bytes[0];
            _buffer[_writePosition + 1] = bytes[1];
            _buffer[_writePosition + 2] = bytes[2];
            _buffer[_writePosition + 3] = bytes[3];
            _writePosition += 4;
        }

        public float ReadSingle()
        {
            var result = BitConverter.ToSingle(_buffer, _readPosition);
            _readPosition += 4;
            return result;
        }

        public void Write(string value)
        {
            var bytes = Encoding.UTF8.GetBytes(value);
            Write(bytes.Length);
            Array.Copy(bytes, 0, _buffer, _writePosition, bytes.Length);
            _writePosition += bytes.Length;
        }

        public string ReadString()
        {
            var length = ReadInt();
            var result = Encoding.UTF8.GetString(_buffer, _readPosition, length);
            _readPosition += length;
            return result;
        }

        public void Write(Vector2 value)
        {
            Write(value.X);
            Write(value.Y);
        }

        public Vector2 ReadVector2()
        {
            var x = ReadSingle();
            var y = ReadSingle();
            return new Vector2(x, y);
        }
    }
}
