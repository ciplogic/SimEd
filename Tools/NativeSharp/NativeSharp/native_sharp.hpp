#pragma once

#include "native_sharp_primitives.hpp"

#include <vector>

#include "native_rc.hpp"

template <typename T> using Arr = std::vector<T>;
template <typename T> using RefArr = Ref<Arr<T>>;

template <class T> RefArr<T> new_arr(int size) {
    RefArr<T> result = new_ref<Arr<T>>();
    result->resize(size);
    return result;
}

template <typename T> T add(T left, T right) { return left + right; }

template <typename T> T sub(T left, T right) { return left - right; }

template <typename T> T mul(T left, T right) { return left * right; }

template <typename T> T rem(T left, T right) { return left % right; }

inline bool cgt(int left, int right) { return left > right; }

inline bool clt(int left, int right) { return left < right; }

inline bool ceq(int left, int right) { return left == right; }

inline bool brfalse_s(int left) { return !left; }

inline bool brtrue_s(int left) { return left; }

inline int32_t conv_i4(uint32_t left) { return left; }
