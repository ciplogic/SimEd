#pragma once

#include "native_sharp_primitives.hpp"

#include <memory>

template<typename T>
using Ref = std::shared_ptr<T>;

struct System_String {
};

Ref<System_String> _clr_str(int index);
