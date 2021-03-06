﻿using UnityEngine;
using System.Collections;
using System;
namespace Lorance.Util {
	public abstract class Option<T> {
		public void Foreach (Action<T> act) {
			if (!IsEmpty ()) {
				act (Get());
			}
		}

		public Option<R> Map<R>(Func<T, R> func) {
			if (IsEmpty ())
				return None<R>.Apply;
			else 
				return new Some<R>(func (Get()));
		}

		public Option<R> FlatMap<R>(Func<T, Option<R>> func) {
			if (IsEmpty ())
				return None<R>.Apply;
			else 
				return func (Get());
		}
			
		public abstract bool IsEmpty();
		public abstract T Get ();

		public static Option<T> Apply(T value) {
			if (value != null) {
				return new Some<T> (value);
			} else {
				return None<T>.Apply;
			}
		}
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

		public override string ToString() {
			return String.Format ("Some({0})", value);
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
		private None(){}

		public override bool IsEmpty() {return true;}
		public override T Get() {
			throw new NoSuchElementException ("None.Get()");
		}

		public static new None<T> Apply = new None<T>();

		public override string ToString() {
			return "None";
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