#pragma once

#include "native_sharp_primitives.hpp"

#include <memory>
#include <vector>

template <typename T>
using Ref = std::shared_ptr<T>;

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
