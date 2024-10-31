#include <iostream>
#include <fstream>
#include "shash.hpp"
#include <sstream>

template <typename T>
struct Option {
    T value;
    bool isSome;
    bool isErr;
    char* errorMessage;
};

template <typename T>
Option<T> Some(T value) {
    return {value, true, false};
}

template <typename T>
Option<T> None() {
    return {{}, false, false};
}
template <typename T>
Option<T> Err(char* message) {
    return {{}, false, true, message};
}

template <typename T>
void printErr(Option<T> option) {
    if (option.isErr) {
        printf(option.errorMessage);
        delete[] option.errorMessage;
    }
}

Option<std::string> ReadAllText(const std::string& path) {
    std::ifstream file(path);

    if (!file.is_open()) {
        char* message = "Could not open file\n";
        return Err<std::string>(message);
    }

    std::stringstream buffer;
    buffer << file.rdbuf();

    std::string fileContent = buffer.str();

    return Some<std::string>(fileContent);
}

int main() {
    Option<std::string> fileContent = ReadAllText("Build.yaml");
    if (fileContent.isErr) {
        printErr(fileContent);
        return 1;
    }
    std::string content = fileContent.value;
    std::string line = "";

    std::string p_language = "";
    std::string p_input_file = "";
    std::string p_output = "";
    std::string p_run_after_build = "";

    char c = content[0];
    int i = 0;
    while (true) {
        c = content[i];
        if (c == '\n') {
            std::string spLine0 = line.substr(0, line.find(": "));
            std::string spLine1 = line.substr(line.find(": ") + 2);
            if (spLine0 == "language") {
                p_language = spLine1;
            } else if (spLine0 == "input") {
                p_input_file = spLine1;
            } else if (spLine0 == "output") {
                p_output = spLine1;
            } else if (spLine0 == "run-after-build") {
                p_run_after_build = spLine1;
            }
            line = "";
        } else {
            line += c;
        }
        i++;
        if (c == '\0') {
            break;
        }
    }
    if (p_language == "" || p_input_file == "" || p_output == "") {
        printf("Invalid Build.yaml file\n");
        return 1;
    }
    if (p_language == "c++") {
        char buildCommand[500];
        sprintf(buildCommand, "g++ -std=c++17 -c %s -o %s", p_input_file.c_str(), p_output.c_str());
        printf(buildCommand);
        std::cout << std::endl;
        system(buildCommand);
        if (p_run_after_build == "true") {
            system(p_output.c_str());
        }
    }
    else if (p_language == "c") {
        char buildCommand[500];
        sprintf(buildCommand, "gcc -c %s -o %s", p_input_file.c_str(), p_output.c_str());
        printf(buildCommand);
        std::cout << std::endl;
        system(buildCommand);
        if (p_run_after_build == "true") {
            system(p_output.c_str());
        }
    }
    else {
        printf("Invalid language\n");
        return 1;
    }
    std::cout << "Build successful\n";
    int data_to_hash = 1;
    for (int i = 0; i < p_input_file.length(); i++) {
        data_to_hash ^= p_input_file[i];
    }
    for (int i = 0; i < p_language.length(); i++) {
        data_to_hash ^= p_language[i];
    }
    for (int i = 0; i < p_output.length(); i++) {
        data_to_hash ^= p_output[i];
    }
    data_to_hash ^= p_run_after_build == "true" ? 0x50 : 0;
    std::cout << "Build config hash: " << Shash::calculate_shash(data_to_hash) << std::endl;
    return 0;
}