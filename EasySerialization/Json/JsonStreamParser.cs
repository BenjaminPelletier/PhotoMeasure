using System.IO;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EasySerialization.Json
{
    public class JsonStreamParser
    {
        private Stream _stream;
        private char? _OnDeck = null;
        private byte[] _Buffer;
        private int _Cursor = 0;
        private int _BufferContentLength = 0;
        private readonly int _BufferSize;

        private RingQueue<string> _Events = null; // new RingQueue<string>(250);

        private const string WHITESPACE = " \t\r\n";
        private const string ESCAPES = "bfnrt\"\\";
        private const string REPLACEMENTS = "\b\f\n\r\t\"\\";
        private const string NUMBER_STARTS = "-0123456789.";

        public JsonStreamParser(Stream stream, int bufferSize = 4096)
        {
            _stream = stream;
            _Buffer = new byte[bufferSize];
            _BufferSize = bufferSize;

            _Events?.Enqueue("Constructor");
        }

        public int Cursor { get { return _Cursor; } }

        private char ReadNextChar()
        {
            if (_OnDeck.HasValue)
            {
                char result = _OnDeck.Value;
                _OnDeck = null;
                return result;
            }

            if (_Cursor >= _BufferContentLength)
            {
                _BufferContentLength = _stream.Read(_Buffer, 0, _Buffer.Length);
                if (_BufferContentLength == 0)
                {
                    throw new EndOfStreamException();
                }
                _Cursor = 0;
            }

            return (char)_Buffer[_Cursor++];
        }

        private void UnreadChar(char c)
        {
            if (_OnDeck.HasValue)
                throw new Exception("Cannot unread two consecutive characters");
            _OnDeck = c;
        }

        private char ReadSkippingWhiteSpace()
        {
            char c;
            do
            {
                c = ReadNextChar();
            } while (WHITESPACE.Contains(c));
            return c;
        }

        private Dictionary<string, JsonObject> ReadDictionary()
        {
            _Events?.Enqueue("ReadDictionary Start");
            var result = new Dictionary<string, JsonObject>();

            char c = ReadSkippingWhiteSpace();
            if (c == '}')
            {
                _Events?.Enqueue("ReadDictionary Blank");
                return result;
            }
            UnreadChar(c);

            while (true)
            {
                KeyValuePair<string, JsonObject> kvp = ParseKeyValuePair();
                result[kvp.Key] = kvp.Value;
                c = ReadSkippingWhiteSpace();
                if (c == '}')
                {
                    _Events?.Enqueue("ReadDictionary End " + result.Keys.Aggregate((a, b) => a + "," + b));
                    return result;
                }
                else if (c != ',')
                    throw new FormatException("Unexpected key-value-pair delimiter in JSON object: '" + c + "'");
            }
        }

        private KeyValuePair<string, JsonObject> ParseKeyValuePair()
        {
            _Events?.Enqueue("ParseKeyValuePair Start");
            string key;
            char c = ReadSkippingWhiteSpace();
            if (c == '"')
            {
                key = ReadString();
            }
            else
            {
                throw new FormatException("Unsupported key format; expected quoted string, found instead '" + c + "'");
            }

            c = ReadSkippingWhiteSpace();
            if (c != ':')
                throw new FormatException("Expected key-value pair separated by ':', found instead '" + c + "' separator");

            JsonObject value = ReadObject();

            _Events?.Enqueue("ParseKeyValuePair End " + key);
            return new KeyValuePair<string, JsonObject>(key, value);
        }

        public JsonObject ReadObject()
        {
            char c = ReadSkippingWhiteSpace();
            if (c == '{')
            {
                _Events?.Enqueue("ReadObject Dictionary");
                return new JsonObject(ReadDictionary());
            }
            else if (c == '"')
            {
                _Events?.Enqueue("ReadObject String");
                return new JsonObject(ReadString());
            }
            else if (c == 't')
            {
                _Events?.Enqueue("ReadObject true");
                ReadSequence("rue");
                return new JsonObject(true);
            }
            else if (c == 'f')
            {
                _Events?.Enqueue("ReadObject false");
                ReadSequence("alse");
                return new JsonObject(false);
            }
            else if (NUMBER_STARTS.Contains(c))
            {
                _Events?.Enqueue("ReadObject Number");
                return new JsonObject(ReadNumber(c));
            }
            else if (c == 'n')
            {
                _Events?.Enqueue("ReadObject null");
                ReadSequence("ull");
                return JsonObject.Null;
            }
            else if (c == '[')
            {
                _Events?.Enqueue("ReadObject Array");
                return new JsonObject(ReadArray());
            }
            else
            {
                _Events?.Enqueue("ReadObject Invalid");
                throw new FormatException("Invalid JSON object starting with '" + c + "'");
            }
        }

        private string ReadString()
        {
            _Events?.Enqueue("ReadString Start");
            var sb = new StringBuilder();
            bool escape = false;
            while (true)
            {
                char c = ReadNextChar();
                if (escape)
                {
                    int i = ESCAPES.IndexOf(c);
                    if (i >= 0)
                    {
                        sb.Append(REPLACEMENTS[i]);
                        escape = false;
                    }
                    else
                    {
                        throw new FormatException("Invalid escape character '" + c + "'");
                    }
                }
                else if (c == '\\')
                {
                    escape = true;
                }
                else if (c == '"')
                {
                    _Events?.Enqueue("ReadString End " + sb.ToString());
                    return sb.ToString();
                }
                else
                {
                    sb.Append(c);
                }
            }
        }

        private void ReadSequence(string sequence)
        {
            foreach (char c in sequence)
            {
                if (ReadNextChar() != c)
                    throw new FormatException("Expected '" + c + "' in sequence '" + sequence + "'");
            }
        }

        private double ReadNumber(char c0)
        {
            _Events?.Enqueue("ReadNumber Start " + c0);
            bool hasDecimal = false;
            const string NUMBERS = "0123456789.";
            var number = new StringBuilder();
            char c = c0;

            bool hasExponent = false;
            bool exponentSign = false;

            while (true)
            {
                if (c == '.')
                {
                    if (hasDecimal)
                        throw new FormatException("Invalid number; only one decimal point is allowed");
                    else
                        hasDecimal = true;
                }

                number.Append(c);

                try
                {
                    c = ReadNextChar();
                }
                catch (EndOfStreamException)
                {
                    break;
                }

                if (c == 'e' || c == 'E')
                {
                    if (hasExponent)
                        throw new FormatException("A number may not have two exponents");
                    hasExponent = true;
                    exponentSign = true;
                }
                else if (c == '-' || c == '+')
                {
                    if (!exponentSign)
                        throw new FormatException("A + or - sign may only be included in a number following the exponent");
                    exponentSign = false;
                }
                else if (!NUMBERS.Contains(c))
                {
                    UnreadChar(c);
                    break;
                }
                else
                {
                    exponentSign = false;
                }
            }

            _Events?.Enqueue("ReadNumber End " + number.ToString());
            return double.Parse(number.ToString());
        }

        private List<JsonObject> ReadArray()
        {
            _Events?.Enqueue("ReadArray Start");
            var result = new List<JsonObject>();

            char c = ReadSkippingWhiteSpace();
            if (c == ']')
            {
                _Events?.Enqueue("ReadArray Blank");
                return result;
            }
            UnreadChar(c);

            while (true)
            {
                JsonObject value = ReadObject();
                result.Add(value);
                c = ReadSkippingWhiteSpace();
                if (c == ']')
                {
                    _Events?.Enqueue("ReadArray End " + result.Count);
                    return result;
                }
                else if (c != ',')
                    throw new FormatException("Unexpected array delimiter in JSON object: '" + c + "'");
            }
        }
    }
}
