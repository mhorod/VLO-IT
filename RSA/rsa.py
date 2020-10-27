import secrets
import base64


class Key:
    def __init__(self, exponent, modulus):
        self.exponent = exponent
        self.modulus = modulus

    def __str__(self):
        return f"({self.exponent}, {self.modulus})"


def random_prime(bits=256):
    attempt = 0
    value = secrets.randbits(bits)
    value |= (1 << (bits - 1)) + 1  # Set up first and last bit
    while True:
        attempt += 1
        if is_prime(value):
            return value

        value += 2


def is_prime(value):
    """Check whether given value is prime using repeated Rabin-Miller algorithm"""
    accuracy = 10
    for _ in range(accuracy):
        if not seems_prime(value):
            return False
    return True


def seems_prime(value):
    """Check whether given value seems prime using Rabin-Miller algorithm"""
    if value == 2: return True
    if value % 2 == 0: return False

    lsb = find_lsb(value - 1)
    d = (value - 1) // (1 << lsb)

    # a is a witness or a liar for compositness of value
    a = 2 + secrets.randbelow(value - 2)

    a = fast_power(a, d, value)
    if a == 1:
        return True

    for _ in range(lsb):
        if a == value - 1:
            return True
        a = (a**2) % value

    return False


def find_lsb(value):
    """Return least significant bit set up"""
    lsb = 0
    while value % 2 == 0:
        lsb += 1
        value >>= 1
    return lsb


def find_msb(value):
    """Return most significant bit set up"""
    return value.bit_length() - 1


def fast_power(base, power, modulus):
    result = 1
    current_bit = find_msb(power)
    while current_bit >= 0:
        result = result**2
        if power & (1 << current_bit):
            result *= base
        current_bit -= 1
        result %= modulus

    return result % modulus


def gcd(a, b):
    while b != 0:
        a, b = b, a % b
    return a


def egcd(a, b):
    """ 
    Given integers a, b return (gcd(a, b), x, y), such that ax + by = gcd(a, b)
    """
    if b == 0:
        return (a, 1, 0)
    else:
        g, x, y = egcd(b, a % b)
        return (g, y, x - (a // b) * y)


def generate_keys(bits=256):
    """Generate (public, private) rsa keys. """
    p = random_prime(bits)
    q = random_prime(bits - 1)  # So that p != q
    N = p * q
    totient = (p - 1) * (q - 1)
    while True:
        e = secrets.randbelow(totient)
        if gcd(e, totient) == 1:
            break

    _, d, _ = egcd(e, totient)
    d %= totient
    return Key(e, N), Key(d, N)


def encrypt_integer(value, key):
    return fast_power(value, key.exponent, key.modulus)


def encrypt_array(values, key):
    return [encrypt_integer(value, key) for value in values]


def join_while_less(byte_data, value):
    current_chunk = b''
    for byte in byte_data:
        byte = bytes([byte])
        if int.from_bytes(current_chunk + byte, 'big') <= value:
            current_chunk += byte
        else:
            yield current_chunk
            current_chunk = byte

    if current_chunk: yield current_chunk


def encrypt_bytes(data, key):
    chunk_size = key.modulus.bit_length() // 8
    return encrypt_partitioned_bytes(partition_bytes(data, chunk_size), key)


def decrypt_bytes(data, key):
    return departiton_bytes(encrypt_partitioned_bytes(data, key))


def encrypt_partitioned_bytes(data, key):
    result = b''
    for chunk in paritioned_chunks(data):
        chunk_value = int.from_bytes(chunk, 'little')
        encrypted_chunk_value = encrypt_integer(chunk_value, key)
        chunk_size = (encrypted_chunk_value.bit_length() + 7) // 8
        encrypted_chunk = encrypted_chunk_value.to_bytes(chunk_size, 'little')
        result += bytes([chunk_size]) + encrypted_chunk

    return result


def partition_bytes(data, chunk_size):
    """Partition byte data into chunks (chunk_size, chunk_data)"""
    result = b''
    for i in range(0, len(data), chunk_size):
        chunk = data[i:i + chunk_size]
        result += bytes([len(chunk)]) + chunk
    return result


def departiton_bytes(data):
    """Remove chunk_size information and convert partitioned data into original whole"""
    result = b''
    for chunk in paritioned_chunks(data):
        result += chunk
    return result


def paritioned_chunks(paritioned_data):
    """Return list of chunks from paritioned_data"""
    chunks = []
    i = 0
    while i < len(paritioned_data):
        chunk_size = paritioned_data[i]
        chunk_data = paritioned_data[i + 1:i + chunk_size + 1]
        chunks.append(chunk_data)
        i += chunk_size + 1
    return chunks


def encrypt_text(text, key, encoding="ascii"):
    text_bytes = encode_text_to_bytes(text, encoding)
    b = encrypt_bytes(text_bytes, key)
    return encode_bytes_to_text(b, encoding)


def decrypt_text(text, key, encoding="ascii"):
    text_bytes = decode_bytes_from_text(text, encoding)
    b = decrypt_bytes(text_bytes, key)
    return decode_text_from_bytes(b, encoding)


def encode_text_to_bytes(text, encoding='ascii'):
    return base64.b64encode(bytes(text, encoding))


def decode_text_from_bytes(data, encoding='ascii'):
    return str(base64.b64decode(data), encoding)


def encode_bytes_to_text(data, encoding='ascii'):
    return str(base64.b64encode(data), encoding)


def decode_bytes_from_text(text, encoding='ascii'):
    return base64.b64decode(bytes(text, encoding))


def demo():
    """Demo featuring example usage."""

    print("Demonstration of fetures\n")

    public, private = generate_keys(bits=8)
    print(f"Generated keys: {public}, {private}")
    print()

    sample_integer = 42
    encrypted_integer = encrypt_integer(sample_integer, public)
    decrypted_integer = encrypt_integer(encrypted_integer, private)

    print(f"Value to encrypt: {sample_integer}")
    print(f"Encrypted: {encrypted_integer}")
    print(f"Decrypted: {decrypted_integer}")

    print()

    sample_text = "This is a sample text that needs to be split into blocks"
    encrypted_text = encrypt_text(sample_text, public)
    decrypted_text = decrypt_text(encrypted_text, private)

    print(f"Text to encrypt: {sample_text}")
    print(f"Encrypted: {encrypted_text}")
    print(f"Decrypted: {decrypted_text}")
    print()

    sample_bytes = b'Some bytes \x01\xaa\xbb\xcc\xdd\xee\xff'
    encrypted_bytes = encrypt_bytes(sample_bytes, public)
    decrypted_bytes = decrypt_bytes(encrypted_bytes, private)
    print(f"Bytes to encrypt: {sample_bytes}")
    print(f"Encrypted: {encrypted_bytes}")
    print(f"Decrypted: {decrypted_bytes}")
    print()


if __name__ == "__main__":
    demo()
