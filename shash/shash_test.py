import subprocess
for i in range(1699):
    result = subprocess.run(["get_shash.exe"], input=f"{i}\n", text=True, capture_output=True)
    hash_value = result.stdout.strip()
    with open("previous_hashes.txt", "r") as file:
        previous_hashes = file.readlines()
    if hash_value + "\n" in previous_hashes:
        print(f"Hash {hash_value} found in the last hashes.")
    else:
        print(f"Hash {hash_value} is new.")
    with open("previous_hashes.txt", "a") as file:
        file.write(hash_value + "\n")