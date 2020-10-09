# Calculate hash with given algorithm
def hash_message(message):
    result = [ord(letter) for letter in "ALGORYTM"]
    message += '.' * ((8 - len(message) % 8) % 8)
    for i, letter in enumerate(message):
        result[i % 8] = (result[i % 8] + ord(letter)) % 128

    return "".join(chr(65 + value % 26) for value in result), result


def skrot(message):
    msg_hash, _ = hash_message(message)
    return msg_hash


class Key:
    def __init__(self, key, modulus):
        self.key = key
        self.modulus = modulus


def encrypt(array, key):
    return [(key.key * value) % key.modulus for value in array]


def read_messages(messages_filename):
    with open(messages_filename, "r") as f:
        return f.read().split('\n')


def read_signatures(signatures_filename):
    with open(signatures_filename, "r") as f:
        return [[int(value) for value in signature.split(' ')]
                for signature in f.read().split('\n') if signature]


def task_78_1(answer_filename):
    messages = read_messages("wiadomosci.txt")
    message = messages[0]

    filled_length = 8 * ((len(message) + 7) // 8)
    message_hash, S_values = hash_message(message)

    with open(answer_filename, "a") as f:
        f.write("Zadanie 78.1\n")
        f.write(f"a) {filled_length}\n")
        f.write(f"b) {' '.join(str(v) for v in S_values)}\n")
        f.write(f"c) {message_hash}\n\n")


def task_78_2(answer_filename):
    signatures = read_signatures("podpisy.txt")
    key = Key(3, 200)
    decrypted_signatures = [
        "".join(chr(v) for v in encrypt(signature, key))
        for signature in signatures
    ]

    with open(answer_filename, "a") as f:
        f.write("Zadanie 78.2\n")
        for signature in decrypted_signatures:
            f.write(f"{signature}\n")
        f.write("\n")


def task_78_3(answer_filename):
    signatures = read_signatures("podpisy.txt")
    key = Key(3, 200)
    decrypted_signatures = [
        "".join(chr(v) for v in encrypt(signature, key))
        for signature in signatures
    ]

    messages = read_messages("wiadomosci.txt")
    valid = [
        i + 1 for i, message in enumerate(messages)
        if skrot(message) == decrypted_signatures[i]
    ]

    with open(answer_filename, "a") as f:
        f.write("Zadanie 78.3\n")
        f.write(f"{' '.join(str(v) for v in valid)}\n\n")


answer_filename = "wyniki.txt"
open(answer_filename, "w").close()
task_78_1(answer_filename)
task_78_2(answer_filename)
task_78_3(answer_filename)
