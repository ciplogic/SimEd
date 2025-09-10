#pragma once

#include "native_sharp_primitives.hpp"

#include <memory>
#include <vector>

template <typename T>
class Rc {
    T* ptr;
    unsigned* count;

public:
    // Constructor
    explicit Rc(T* p = nullptr) : ptr(p), count(new unsigned(1)) {}

    // Copy constructor
    Rc(const Rc& other) : ptr(other.ptr), count(other.count) {
        ++(*count);
    }

    // Assignment operator
    Rc& operator=(const Rc& other) {
        if (this != &other) {
            release();
            ptr = other.ptr;
            count = other.count;
            ++(*count);
        }
        return *this;
    }

    // Destructor
    ~Rc() {
        release();
    }

    // Dereference operators
    T& operator*() const { return *ptr; }
    T* operator->() const { return ptr; }

    // Access reference count
    unsigned use_count() const { return *count; }

private:
    void release() {
        if (--(*count) == 0) {
            delete ptr;
            delete count;
        }
    }
};


template <typename T>
//using Ref = std::shared_ptr<T>;
using Ref = Rc<T>;

template <typename T>
using Arr = std::vector<T>;

template <typename T>
using RefArr = Ref<Arr<T>>;

inline int add(int left, int right) {
    return left + right;
}
inline int sub(int left, int right) {
    return left - right;
}

inline int mul(int left, int right) {
    return left * right;
}
inline int rem(int left, int right) {
    return left % right;
}

inline bool cgt(int left, int right) {
    return left > right;
}

inline bool clt(int left, int right) {
    return left < right;
}

inline bool ceq(int left, int right) {
    return left == right;
}
inline bool brfalse_s(bool left) {
    return !left;
}
inline bool brtrue_s(bool left) {
    return left;
}

inline int32_t conv_i4(uint32_t left) {
    return left;
}
