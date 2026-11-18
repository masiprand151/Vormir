type StateUpdater<T> = T | ((previous: T) => T);

type StateSetter<T> = (value: StateUpdater<T>) => void;

const hookStates: unknown[] = [];

let hookIndex = 0;
let isRendering = false;
let rerenderApp: (() => void) | undefined;

export function beginHooks(): void {
  hookIndex = 0;
  isRendering = true;
}

export function endHooks(): void {
  isRendering = false;
}

export function setHookRenderer(callback: () => void): void {
  rerenderApp = callback;
}

export function useState<T>(initialValue: T | (() => T)): [T, StateSetter<T>] {
  if (!isRendering) {
    throw new Error("useState must be called while rendering an app.");
  }

  const currentIndex = hookIndex++;

  if (!(currentIndex in hookStates)) {
    hookStates[currentIndex] =
      typeof initialValue === "function"
        ? (initialValue as () => T)()
        : initialValue;
  }

  const setState: StateSetter<T> = (value) => {
    const previous = hookStates[currentIndex] as T;

    const next =
      typeof value === "function"
        ? (value as (previous: T) => T)(previous)
        : value;

    if (Object.is(previous, next)) {
      return;
    }

    hookStates[currentIndex] = next;

    rerenderApp?.();
  };

  return [hookStates[currentIndex] as T, setState];
}
