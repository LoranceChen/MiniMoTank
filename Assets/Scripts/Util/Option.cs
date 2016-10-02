﻿using UnityEngine;
using System.Collections;
using Functional;
//using Lorance;
namespace Lorance.RxScoket.Util {
	public abstract class Option<T> {
		public void Foreach (Action<T> act) {
			if (!IsEmpty ()) {
				act (Get());
			}
		}

		public Option<R> Map<R>(Func<T, R> func) {
			if (IsEmpty ())
				return new None<R> ();
			else 
				return new Some<R>(func (Get()));
		}

		public Option<R> FlatMap<R>(Func<T, Option<R>> func) {
			if (IsEmpty ())
				return new None<R> ();
			else 
				return func (Get());
		}
			
		public abstract bool IsEmpty();
		public abstract T Get ();
	}

//	public static class IOptionEx {
//		public static T GetOrElse<T> (this IOption<T> opt, Func<T> defaultValue) {
//			return opt.IsEmpty() ? defaultValue() : opt.Get();
//		}
//	}

	public class Some<T> : Option<T>{
		T value;

		public Some(T value){
			this.value = value;
		}

		public override bool IsEmpty() {return false;}

		public override T Get() {
			return value;
		}
//
//		public void Foreach (Action<T> act) {
//			if (!IsEmpty ()) {
//				act (Get());
//			}
//		}
//
//		public Option<R> Map<R>(Func<T, R> func) {
//			if (IsEmpty ())
//				return new None<R> ();
//			else 
//				return new Some<R>(func (Get()));
//		}
//			
//		public Option<R> FlatMap<R>(Func<T, Option<R>> func) {
//			if (IsEmpty ())
//				return new None<R> ();
//			else 
//				return func (Get());
//		}

//		public static Option<T> apply(T value){
//			if (value == null)
//				return new None<T> ();
//			else
//				return new Some<T> (value);
//		}

//		public static Option<T> Empty = new None<T>();
	}

	public class None<T> : Option<T> {
		public None(){}

		public override bool IsEmpty() {return true;}
		public override T Get() {
			throw new NoSuchElementException ("None.Get()");
		}

//		public void Foreach (Action<T> act) {
//			if (!IsEmpty ()) {
//				act (Get());
//			}
//		}
//
//		public Option<R> Map<R>(Func<T, R> func) {
//			if (IsEmpty ())
//				return new None<R> ();
//			else 
//				return new Some<R>(func (Get()));
//		}
//
//		public Option<R> FlatMap<R>(Func<T, Option<R>> func) {
//			if (IsEmpty ())
//				return new None<R> ();
//			else 
//				return func (Get());
//		}
//
//		public static Option<T> apply(T value){
//			if (value == null)
//				return new None<T> ();
//			else
//				return new Some<T> (value);
//		}
//
//		public static Option<T> Empty = new None<T>();
	}

	public class NoSuchElementException : System.SystemException {
		public NoSuchElementException(string message)
			: base(message)
		{
		}
	}
}