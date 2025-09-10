#pragma once

#define OPTIMIZED_RC

#ifndef OPTIMIZED_RC
#include <memory>
#else

template <typename T> struct RcData {
    int _count = 1;
    int _typeId;
    T _data;
};

template <typename T> class Rc {
    RcData<T> *_data;

public:
    // Constructor
    explicit Rc(RcData<T> *p = nullptr) : _data(p) {}

    // Copy constructor
    Rc(const Rc &other) : _data(other._data) { ++_data->_count; }

    // Assignment operator
    Rc &operator=(const Rc &other) {
        if (this != &other) {
            release();
            _data = other._data;
            ++_data->_count;
        }
        return *this;
    }

    // Destructor
    ~Rc() { release(); }

    // Dereference operators
    T &operator*() const { return _data->_data; }
    T *operator->() const { return &_data->_data; }
    T *get() { return &_data->_data; }
    T *get() const { return &_data->_data; }
    // Access reference count
    int use_count() const { return _data->_count; }

private:
    void release() {
        if (!_data) {
            return;
        }
        --_data->_count;
        if (!_data->_count) {
            delete _data;
        }
    }
};
#endif

template <typename T>
using Ref =
#ifdef OPTIMIZED_RC
    Rc<T>;
#else
        std::shared_ptr<T>;
#endif

template <typename T> Ref<T> new_ref(int typeId = 0) {
#ifdef OPTIMIZED_RC
    RcData<T> *data = new RcData<T>();
    data->_typeId = typeId;
    return Ref(data);
#else
    return std::make_shared<T>();
#endif
}

template <typename T> Ref<T> new_ref_data(const T &dataItem, int typeId = 0) {
#ifdef OPTIMIZED_RC
    auto *data = new RcData<T>();
    data->_data = dataItem;
    data->_typeId = typeId;
    return Ref<T>(data);
#else
    return std::make_shared<T>(dataItem);
#endif
}
