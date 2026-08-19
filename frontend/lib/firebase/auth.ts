import {
  createUserWithEmailAndPassword,
  signInWithEmailAndPassword,
  signInWithPopup,
  GoogleAuthProvider,
  signOut,
  onAuthStateChanged,
  type User,
} from "firebase/auth";
import { auth } from "./client";

const googleProvider = new GoogleAuthProvider();

function requireAuth() {
  if (!auth) {
    throw new Error(
      "Firebase is not configured — set NEXT_PUBLIC_FIREBASE_* env vars in .env.local"
    );
  }
  return auth;
}

export function registerWithEmail(email: string, password: string) {
  return createUserWithEmailAndPassword(requireAuth(), email, password);
}

export function loginWithEmail(email: string, password: string) {
  return signInWithEmailAndPassword(requireAuth(), email, password);
}

export function loginWithGoogle() {
  return signInWithPopup(requireAuth(), googleProvider);
}

export function logout() {
  return signOut(requireAuth());
}

export function subscribeToAuthChanges(callback: (user: User | null) => void) {
  if (!auth) {
    callback(null);
    return () => {};
  }
  return onAuthStateChanged(auth, callback);
}

export function getIdToken(user: User) {
  return user.getIdToken();
}
