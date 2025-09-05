#pragma once

#include "native_sharp_primitives.hpp"

#include <memory>
#include <vector>

template <typename T>
using Ref = std::shared_ptr<T>;

template <typename T>
using RefArr = Ref<std::vector<T>>;

struct System_String {
};

Ref<System_String> _clr_str(int index);


inline int add(int left, int right) {
    return left + right;
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

inline bool ceq(int left, int right) {
    return left == right;
}
inline bool brfalse_s(bool left) {
    return !left;
}
inline bool brtrue_s(bool left) {
    return left;
}