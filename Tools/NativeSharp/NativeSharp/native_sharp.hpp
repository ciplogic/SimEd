#pragma once

#include "native_sharp_primitives.hpp"

#include <memory>
#include <vector>

template<typename T>
struct RcData
{
    int _count = 1;
    int _typeId;
    T _data;
};

template<typename T>
class Rc
{
    RcData<T>* _data;

    public:
        // Constructor
        explicit Rc(RcData<T>* p = nullptr) : _data(p)
        {
        }

        // Copy constructor
        Rc(const Rc& other) : _data(other._data)
        {
            ++_data->_count;
        }

        // Assignment operator
        Rc& operator=(const Rc& other)
        {
            if (this != &other)
            {
                release();
                _data = other._data;
                ++_data->_count;
            }
            return *this;
        }

        // Destructor
        ~Rc()
        {
            release();
        }

    // Dereference operators
    T& operator*() const { return _data->_data; }
    T* operator->() const { return &_data->_data; }
    T* get() { return &_data->_data; }
    T* get() const { return &_data->_data; }
    // Access reference count
    int use_count() const { return _data->_count; }

    private:
        void release()
        {
            if (!_data)
            {
                return;
            }
            --_data->_count;
            if (!_data->_count)
            {
                delete _data;
            }
        }
};

template<typename T>
//using Ref = std::shared_ptr<T>;
using Ref = Rc<T>;

template<typename T>
Ref<T> new_ref(int typeId = 0)
{
    RcData<T>* data = new RcData<T>();
    data->_count = 1;
    data->_typeId = typeId;
    return Ref(data);
}

template<typename T>
Ref<T> new_ref_data(T& dataItem, int typeId = 0)
{
    auto* data = new RcData<T>();
    data->_data = dataItem;
    data->_count = 1;
    data->_typeId = typeId;
    return Ref<T>(data);
}

template<typename T>
Ref<T> new_ref_data(const T& dataItem, int typeId = 0)
{
    auto* data = new RcData<T>();
    data->_data = dataItem;
    data->_count = 1;
    data->_typeId = typeId;
    return Ref<T>(data);
}

template<typename T>
using Arr = std::vector<T>;

template<typename T>
using RefArr = Ref<Arr<T> >;

template<typename T>
T add(T left, T right)
{
    return left + right;
}

template<typename T>
T sub(T left, T right)
{
    return left - right;
}

template<typename T>
T mul(T left, T right)
{
    return left * right;
}

template<typename T>
T rem(T left, T right)
{
    return left % right;
}

template<typename T>
bool cgt(T left, T right)
{
    return left > right;
}

template<typename T>
bool clt(T left, T right)
{
    return left < right;
}

template<typename T>
bool ceq(T left, T right)
{
    return left == right;
}

bool brfalse_s(bool left)
{
    return !left;
}

bool brtrue_s(bool left)
{
    return left;
}


template<typename T>
int32_t conv_i4(uint32_t left)
{
    return left;
}
