import secrets


class Key:
    def __init__(self, exponent, modulus):
        self.exponent = exponent
        self.modulus = modulus


def random_prime(bits=256):
    attempt = 0
    value = secrets.randbits(bits)
    value |= (1 << (bits - 1)) + 1
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
    return len(bin(value)) - 2


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
    if b == 0:
        return a
    return gcd(b, a % b)


def egcd(a, b):
    """ 
    Given integers a, b return (gcd(a, b), x, y), such that ax + by = gcd(a, b)
    """
    if b == 0:
        return (a, 1, 0)
    else:
        g, x, y = egcd(b, a % b)
        return (g, y, x - (a // b) * y)


def generate_keys():
    """Generate public and private RSA keys"""
    p = random_prime(8)
    q = random_prime(8)
    N = p * q
    totient = (p - 1) * (q - 1)
    while True:
        e = secrets.randbelow(totient)
        if gcd(e, totient) == 1:
            break

    _, d, _ = egcd(e, totient)
    d %= totient
    return Key(e, N), Key(d, N)


def encode(value, key):
    return fast_power(value, key.exponent, key.modulus)


if __name__ == '__main__':
    public, private = generate_keys()
    value = 12345
    v1 = encode(value, public)
    v2 = encode(v1, private)
    print(v1, v2)
