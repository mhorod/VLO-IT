import random

import pytest

import rsa


def test_fast_power():
    repeat_count = 20
    for _ in range(repeat_count):
        base = random.randint(1, 1000)
        exponent = random.randint(1, 1000)
        modulus = random.randint(1, 100000)
        assert rsa.fast_power(base, exponent,
                              modulus) == (base**exponent) % modulus


def test_is_prime():
    primes = [3, 5, 7, 11, 17]
    for prime in primes:
        assert rsa.is_prime(prime)


def test_is_not_prime():
    values = [6, 8, 10, 100, 120]
    for value in values:
        assert not rsa.is_prime(value)


def test_gcd():
    v1 = 2**10 * 15
    v2 = 2**20 * 5
    assert rsa.gcd(v1, v2) == 2**10 * 5


def test_egcd():
    v1 = 17
    v2 = 13
    g, x, y = rsa.egcd(v1, v2)
    assert g == 1 and (v1 * x + v2 * y) == g


def test_encrypted_integer_can_be_decrypted():
    repeat_count = 10
    for _ in range(repeat_count):
        public, private = rsa.generate_keys()
        value = random.randint(1, public.modulus - 1)
        assert rsa.encrypt_integer(rsa.encrypt_integer(value, public),
                                   private) == value


def test_encrypted_bytes_can_be_decrypted():
    public, private = rsa.generate_keys()
    data_size = 256
    data = bytes([random.randint(0, 255) for _ in range(data_size)])
    assert data == rsa.decrypt_bytes(rsa.encrypt_bytes(data, public), private)


def test_encrypted_text_can_be_encrypted():
    public, private = rsa.generate_keys()
    text_length = 256
    text = ''.join(chr(random.randint(32, 125)) for _ in range(text_length))
    assert text == rsa.decrypt_text(rsa.encrypt_text(text, public), private)
