import pytest

import rsa


def test_fast_power():
    assert rsa.fast_power(38, 107, 215) == 197


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
