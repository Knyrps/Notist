import { BackendService } from "./backendService";

export class AppVersion {
    public major: number;
    public minor: number;
    public build: number;

    toString = () => {
        return `${this.major}.${this.minor}.${this.build}`;
    };

    constructor(version: string);
    constructor(major: number, minor?: number, build?: number);

    constructor(
        versionOrMajor: string | number,
        minor?: number,
        build?: number
    ) {
        if (typeof versionOrMajor === "string") {
            const parsed = versionOrMajor
                .split(".")
                .map((n) => parseInt(n, 10));
            this.major =
                Number.isFinite(parsed[0]) && parsed[0] >= 0 ? parsed[0] : 0;
            this.minor =
                Number.isFinite(parsed[1]) && parsed[1] >= 0 ? parsed[1] : 0;
            this.build =
                Number.isFinite(parsed[2]) && parsed[2] >= 0 ? parsed[2] : 0;
        } else {
            this.major = versionOrMajor;
            this.minor = minor ?? 0;
            this.build = build ?? 0;
        }
    }

    static Empty = Object.freeze(new AppVersion(0, 0, 0));
}

let version: AppVersion | undefined = undefined;

export default async function appVersion(): Promise<AppVersion> {
    if (version !== undefined) {
        return version;
    }
    const mmb: string = await BackendService.getVersion();
    version = new AppVersion(mmb);
    return version;
}
